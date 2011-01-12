from socket import *

addr = ("10.195.209.26", 1024)
UDPSock = socket(AF_INET, SOCK_DGRAM)

msg = "bazinga!"

UDPSock.sendto(msg, addr)
