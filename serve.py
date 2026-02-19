#!/usr/bin/env python3
"""Simple HTTP server with gzip Content-Encoding for Unity WebGL builds."""
import http.server
import os

class UnityWebGLHandler(http.server.SimpleHTTPRequestHandler):
    def __init__(self, *args, **kwargs):
        super().__init__(*args, directory="docs", **kwargs)

    def end_headers(self):
        path = self.translate_path(self.path)
        if path.endswith('.gz'):
            self.send_header('Content-Encoding', 'gzip')
            if path.endswith('.js.gz'):
                self.send_header('Content-Type', 'application/javascript')
            elif path.endswith('.wasm.gz'):
                self.send_header('Content-Type', 'application/wasm')
            elif path.endswith('.data.gz'):
                self.send_header('Content-Type', 'application/octet-stream')
        super().end_headers()

if __name__ == '__main__':
    port = 8888
    server = http.server.HTTPServer(('', port), UnityWebGLHandler)
    print(f"Serving Unity WebGL at http://localhost:{port}")
    server.serve_forever()
