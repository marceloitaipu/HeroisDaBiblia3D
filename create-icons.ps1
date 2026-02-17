# Script para criar ícones PNG para PWA
# Cria ícones temporários com gradiente e texto

Add-Type -AssemblyName System.Drawing

function Create-Icon {
    param(
        [int]$size,
        [string]$outputPath
    )
    
    # Criar bitmap
    $bmp = New-Object System.Drawing.Bitmap($size, $size)
    $graphics = [System.Drawing.Graphics]::FromImage($bmp)
    $graphics.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::AntiAlias
    
    # Criar gradiente roxo/azul
    $rect = New-Object System.Drawing.Rectangle(0, 0, $size, $size)
    $color1 = [System.Drawing.Color]::FromArgb(102, 126, 234)  # #667eea
    $color2 = [System.Drawing.Color]::FromArgb(118, 75, 162)   # #764ba2
    
    $brush = New-Object System.Drawing.Drawing2D.LinearGradientBrush(
        $rect,
        $color1,
        $color2,
        45
    )
    
    # Preencher fundo com gradiente
    $graphics.FillRectangle($brush, $rect)
    
    # Adicionar bordas arredondadas (círculo)
    $pen = New-Object System.Drawing.Pen([System.Drawing.Color]::White, ($size * 0.02))
    $innerRect = New-Object System.Drawing.Rectangle(
        ($size * 0.1),
        ($size * 0.1),
        ($size * 0.8),
        ($size * 0.8)
    )
    $graphics.DrawEllipse($pen, $innerRect)
    
    # Adicionar texto "HB" no centro
    $fontSize = [int]($size * 0.4)
    $font = New-Object System.Drawing.Font("Arial", $fontSize, [System.Drawing.FontStyle]::Bold)
    $textBrush = New-Object System.Drawing.SolidBrush([System.Drawing.Color]::White)
    
    $text = "HB"
    $textSize = $graphics.MeasureString($text, $font)
    
    $x = ($size - $textSize.Width) / 2
    $y = ($size - $textSize.Height) / 2
    
    $graphics.DrawString($text, $font, $textBrush, $x, $y)
    
    # Salvar
    $bmp.Save($outputPath, [System.Drawing.Imaging.ImageFormat]::Png)
    
    # Limpar recursos
    $graphics.Dispose()
    $bmp.Dispose()
    $brush.Dispose()
    $pen.Dispose()
    $font.Dispose()
    $textBrush.Dispose()
    
    Write-Host "✅ Criado: $outputPath" -ForegroundColor Green
}

# Criar ícones
Write-Host "🎨 Criando ícones PNG..." -ForegroundColor Cyan

Create-Icon -size 192 -outputPath "docs\icon-192.png"
Create-Icon -size 512 -outputPath "docs\icon-512.png"

Write-Host ""
Write-Host "Icones criados com sucesso!" -ForegroundColor Green
Write-Host "Localizados em: docs\icon-192.png e docs\icon-512.png" -ForegroundColor Yellow
