import socket

import click

@click.group()
def cli():
    pass

@cli.command()
@click.option("--size", "-s", help="Entrez la taille des données à écrire", type=int, default=1000)
@click.option("--method", "-m", help="Entrez la méthode d'écriture à utiliser", default="None")
@click.option("--iteration", "-i", help="Entrez le numéro de l'itération", default="first")
@click.option("--maxtx", "-x", help="Entrez le nombre de transaction max", type=int, default=100)
def writetx(size, method, iteration, maxtx):
    # Construire la commande:
    commandBits = []
    commandBits.append("-writeTx")
    commandBits.append(f"-size={size}")
    commandBits.append(f"-method={method}")
    commandBits.append(f"-iteration={iteration}")
    commandBits.append(f"-maxTx={maxtx}")
    
    command = " ".join(commandBits)
    send_command(command)
    
    
@cli.command()
@click.option("--size", "-s", help="Entrez la taille des données à écrire", type=int, default=1000)
@click.option("--method", "-m", help="Entrez la méthode d'écriture à utiliser", default="None")
@click.option("--iteration", "-i", help="Entrez le numéro de l'itération", default="first")
def analyze(size, method, iteration):
    commandBits = []
    commandBits.append("-analyze")
    commandBits.append(f"-size={size}")
    commandBits.append(f"-method={method}")
    commandBits.append(f"-iteration={iteration}")
    
    
    command = " ".join(commandBits)
    send_command(command)

def send_command(command: str, host='192.168.0.141', port=65432):
    # Envoi de la commande
    with socket.socket(socket.AF_INET, socket.SOCK_STREAM) as s:
        s.connect((host, port))
        s.sendall(command.encode('utf-8'))
        # data = s.recv(4096)
        # print(f"\nReceived output:\n{data.decode('iso-8859-1')}\n")

if __name__ == "__main__":
    cli()

