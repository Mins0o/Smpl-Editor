import json
import tkinter as tk
from tkinter import filedialog
import sys
import os

def loadFiles(verbose=False):
    files=[]
    if verbose:
        filePaths = fd.askopenfilenames()
        for i in filePaths:
            files.append(open(i,'r',encoding='UTF-8'))
    else:
        f1=open("D:/Dropbox/Workspace/03 Python/03 Smpl-Editor/Master.txt",'r',encoding='UTF-8')
        f2=open("D:/Dropbox/Workspace/03 Python/03 Smpl-Editor/Master2.smpl",'r',encoding='UTF-8')
        f3=open("D:/Dropbox/Workspace/03 Python/03 Smpl-Editor/Master3.smpl",'r',encoding='UTF-8')
        f4=open("D:/Dropbox/Workspace/03 Python/03 Smpl-Editor/Master4.smpl",'r',encoding='UTF-8')
        files=[f1,f2,f3,f4]
    return files

def display(lines):
    os.system('cls')
    for line in lines:
        print(line)
    
x=json.loads(f1.readline())["members"][:1]
for i in range(len(x)):
    try:
        print(str(i)+" "+str(x[i]))
    except:
        print(i)
