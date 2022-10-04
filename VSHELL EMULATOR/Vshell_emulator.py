import argparse
import os.path
import zipfile

parser = argparse.ArgumentParser(description='This is emulator vshell, he have commands such as pwd, ls, cd, cat')
parser.add_argument("zipfile", type=str,
                    help='Location and name .zip file')
args = parser.parse_args()


def ls(command):
    global CurrentZipPathObject
    if (len(command) == 2):
        CopyPath = CurrentZipPathObject.joinpath(command[1])
        if not (CopyPath.exists()):
            print("Such directory or file is not exist")
            return
        if (CopyPath.is_dir()):
            for iter in CopyPath.iterdir():
                print(iter.name)
            return
        else:
            print("Is not a directory")
            return
    elif (len(command) == 1):
        for iter in CurrentZipPathObject.iterdir():
            print(iter.name)
        return


def cd(command):
    global CurrentPath
    global CurrentZipPathObject
    global PathOfZip
    if len(command) == 2:
        testZipPathObject = CurrentZipPathObject.joinpath(command[1])
        if testZipPathObject.exists() and testZipPathObject.is_dir():
            CurrentPath += "/" + command[1]
            CurrentZipPathObject = testZipPathObject
        else:
            print("Path is not exist or its a file")
    elif len(command) == 1:
        CurrentPath = ""
        CurrentZipPathObject = zipfile.Path(zipfile.ZipFile(PathOfZip))


def cat(command):
    global CurrentPath
    global  CurrentZipPathObject
    global PathOfZip
    if len(command) == 2:
        testZipPathObject = CurrentZipPathObject.joinpath(command[1])
        if testZipPathObject.exists() and testZipPathObject.is_file():
            print(testZipPathObject.read_text())
        else:
            print("Current file is not exist or its directory")

def main():
    global PathOfZip
    global CurrentPath
    global CurrentZipPathObject
    ZipObject = zipfile.ZipFile(PathOfZip)
    CurrentPath = ""
    CurrentZipPathObject = zipfile.Path(ZipObject)
    print("Starting emulator of vshell...")
    while True:
        print(PathOfZip + CurrentPath + " $ ", end='')
        command = input().split()
        if command is None:
            continue
        if command[0] == "ls":
            ls(command)
            continue
        elif command[0] == "pwd":
            print("/root" + CurrentPath)
            continue
        elif command[0] == "cd":
            cd(command)
        elif command[0] == "cat":
            cat(command)


if __name__ == '__main__':
    CurrentPath = ""
    PathOfZip = args.zipfile
    CurrentZipPathObject = zipfile.Path(zipfile.ZipFile(PathOfZip))

    main()
