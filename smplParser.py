import json
f1=open("D:/Dropbox/Workspace/03 Python/03 Smpl-Editor/Master.txt",'r',encoding='UTF-8')
f2=open("D:/Dropbox/Workspace/03 Python/03 Smpl-Editor/Master2.smpl",'r',encoding='UTF-8')
f3=open("D:/Dropbox/Workspace/03 Python/03 Smpl-Editor/Master3.smpl",'r',encoding='UTF-8')
f4=open("D:/Dropbox/Workspace/03 Python/03 Smpl-Editor/Master4.smpl",'r',encoding='UTF-8')

def smplParser(smpl):
    smpl=smpl[13:]
    temp=smpl.index("]",len(smpl)*98//100)
    smpl=smpl[:temp-1]
    musicList=smpl.split("},{")
    musicList2=[]
    for i in musicList[:10]:
        i=i[1:]
        musicList2.append(i.split('","'))
    for i in musicList2:
        print(i)


#smplParser(f1.readline())
x=json.loads(f1.readline())["members"]
for i in range(len(x)):
    try:
        print(str(i)+" "+str(x[i]))
    except:
        print(i)



