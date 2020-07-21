f1=open(r"C:\Users\steng\Desktop\Master.txt",'r',encoding='UTF-8')
f2=open(r"C:\Users\steng\Desktop\Master2.smpl",'r',encoding='UTF-8')
f3=open(r"C:\Users\steng\Desktop\Master3.smpl",'r',encoding='UTF-8')
f4=open(r"C:\Users\steng\Desktop\Master4.smpl",'r',encoding='UTF-8')

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


smplParser(f1.readline())
# No 0% day!



