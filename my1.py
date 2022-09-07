import  argparse
import os.path
import zipfile
parser = argparse.ArgumentParser(description='This is emulator vshell, he have commands such as pwd, ls, cd, cat')
parser.add_argument("zipfile", type=str,
                    help='Location and name .zip file')
parser.add_argument("command", type=str, help="Command name {pwd, ls, cd, cat}")
args = parser.parse_args()


def AddDirrectoryManager(zipPath):
    ZF = zipfile.ZipFile(zipPath, mode='a', compression=zipfile.ZIP_DEFLATED)
    filetext = "/"
    filename = "DirectoryManager.txt"

    if("DirectoryManager.txt" not in ZF.namelist()):
        ZF.writestr(filename, filetext)

def ls(zipPath):
    with zipfile.ZipFile(zipPath) as zf:
        for info in zf.infolist():
            print(info.filename)
def pwd(zipPath):
    ZipFile = zipfile.ZipFile(zipPath)
    byte_text =  ZipFile.read("DirectoryManager.txt")
    print(byte_text.decode('utf-8'))
def main(zipPath):
    if (zip == None):
        print(parser.usage)
        exit(0)
    ZipFile = zipfile.ZipFile(zipPath)


if __name__ == '__main__':
    # USAGE - Project.py -z zipname.zip -f file.txt
    command = args.command
    AddDirrectoryManager(args.zipfile)
    if(command == "pwd"):
        pwd(args.zipfile)
    elif (command == "ls"):
        ls(args.zipfile)