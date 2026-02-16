using System;
using UnityEngine;

namespace HeroisDaBiblia3D
{
    /// <summary>
    /// Modo de jogo para o Mundo 4 - Moisés (Puzzle de ordenação de eventos).
    /// O jogador deve ordenar 4 eventos do milagre do Mar Vermelho na sequência correta.
    /// </summary>
    public sealed class MoisesPuzzleMode : MonoBehaviour
    {
        /// <summary>Callback executado quando o puzzle é completado corretamente.</summary>
        public Action onWin;
        
        /// <summary>Callback executado quando o jogador erra a ordem.</summary>
        public Action onFail;

        private int[] _sequence = new int[4]; // Sequência escolhida pelo jogador
        private int _currentStep = 0; // Índice da próxima escolha
        
        // Ordem correta: 0 (Cajado ao alto) -> 1 (Mar se abre) -> 2 (Povo atravessa) -> 3 (Mar fecha)
        private readonly int[] _correctSequence = { 0, 1, 2, 3 };

        /// <summary>
        /// Reinicia o puzzle, limpando todas as escolhas anteriores.
        /// </summary>
        public void ResetPuzzle()
        {
            _currentStep = 0;
            for (int i = 0; i < _sequence.Length; i++)
                _sequence[i] = -1;
        }

        /// <summary>
        /// Registra uma escolha do jogador no puzzle.
        /// </summary>
        /// <param name="step">Índice do evento escolhido (0-3).</param>
        public void Pick(int step)
        {
            if (_currentStep >= _sequence.Length)
                return;

            // Verifica se esse evento já foi escolhido
            for (int i = 0; i < _currentStep; i++)
            {
                if (_sequence[i] == step)
                    return; // Já foi escolhido, ignora
            }

            _sequence[_currentStep] = step;
            _currentStep++;

            // Verifica se completou o puzzle
            if (_currentStep >= _sequence.Length)
            {
                CheckSolution();
            }
        }

        /// <summary>
        /// Verifica se a sequência escolhida está correta e aciona os callbacks apropriados.
        /// </summary>
        private void CheckSolution()
        {
            bool correct = true;
            for (int i = 0; i < _sequence.Length; i++)
            {
                if (_sequence[i] != _correctSequence[i])
                {
                    correct = false;
                    break;
                }
            }

            if (correct)
                onWin?.Invoke();
            else
                onFail?.Invoke();
        }

        /// <summary>
        /// Retorna uma string representando o estado atual do puzzle para exibição na UI.
        /// </summary>
        /// <returns>String formatada mostrando a sequência atual (ex: "Cajado → Mar abre → — → —").</returns>
        public string StatusText()
        {
            string[] labels = { "Cajado", "Mar abre", "Povo passa", "Mar fecha" };
            string result = "";
            
            for (int i = 0; i < _sequence.Length; i++)
            {
                if (i > 0) result += " → ";
                
                if (i < _currentStep && _sequence[i] >= 0 && _sequence[i] < labels.Length)
                    result += labels[_sequence[i]];
                else
                    result += "—";
            }
            
            return result;
        }
    }
}
