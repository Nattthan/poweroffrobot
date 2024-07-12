import socket
import subprocess
import threading

def execute_command(command):
    try:
        args = command.split()
        method_write_tx = "-writeTx" in args
        method_analyze = "-analyze" in args
        size = next((arg[6:] for arg in args if arg.startswith("-size=")), None)
        method = next((arg[8:] for arg in args if arg.startswith("-method=")), None)
        iteration = next((arg[11:] for arg in args if arg.startswith("-iteration=")), None)

        result = subprocess.run([
            "C:\\Users\\guillotn\\_work\\filesForAtmIp\\x86\\Remote\\WriteTransactions.exe",
            "-writeTx" if method_write_tx else "",
            "-analyze" if method_analyze else "",
            f"-size={size}" if size else "",
            f"-method={method}" if method else "",
            f"-iteration={iteration}" if iteration else ""
        ], shell=True, check=True, stdout=subprocess.PIPE, stderr=subprocess.PIPE)

        return result.stdout + result.stderr

    except subprocess.CalledProcessError as e:
        return str(e)

def handle_client(conn, addr):
    print(f'Connected by {addr}')
    try:
        while True:
            data = conn.recv(1024)
            if not data:
                break
            try:
                command = data.decode('utf-8')
            except UnicodeDecodeError:
                print("Erreur de décodage : la commande reçue n'est pas en UTF-8 valide.")
                conn.sendall("Erreur de décodage".encode('utf-8'))
                continue
            print(f'Received command: {command}')
            result = execute_command(command)
            print(f'result command: {result}')
            #conn.sendall(result.encode('utf-8'))
            conn.sendall(result)
    finally:
        conn.close()

def start_server(host='0.0.0.0', port=65432):
    with socket.socket(socket.AF_INET, socket.SOCK_STREAM) as s:
        s.bind((host, port))
        s.listen()
        print(f'Listening on {host}:{port}')
        while True:
            conn, addr = s.accept()
            client_thread = threading.Thread(target=handle_client, args=(conn, addr))
            client_thread.start()

if __name__ == '__main__':
    start_server()
