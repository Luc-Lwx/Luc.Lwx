

# HINTS TO LOCAL TESTING

### Setup environment with Luc.Lwx and Luc.Lwx.Examples side-by-side
```sh
mkdir ~/Dev/Luc.Lwx
cd ~/Dev/Luc.Lwx
git clone git@github.com:lucas75/Luc.Lwx.git
git clone git@github.com:lucas75/Luc.Lwx.Examples.git
``` 

### Build the local libraries
```sh
cd Luc.Lwx
dotnet clean; dotnet build; dotnet pack --no-restore --no-build -o ../nuget-local
```


# UBUNTU

I recommend to install dotnet from Microsoft, both ubuntu and snap packages have some problems:
* Test explorer doesn't work (the tests are not detected)
* Add nuget package from vscode command bar doesn't work

You just have to download it from:
https://dotnet.microsoft.com/download/dotnet/8.0
(binnary/x64)

```sh
# mkdir ~/bin/dotnet8
# cd ~/bin/dotnet8
# wget https://download.visualstudio.microsoft.com/download/pr/4e3b04aa-c015-4e06-a42e-05f9f3c54ed2/74d1bb68e330eea13ecfc47f7cf9aeb7/dotnet-sdk-8.0.404-linux-x64.tar.gz
...
# sha512sum dotnet-sdk-8.0.404-linux-x64.tar.gz 
2f166f7f3bd508154d72d1783ffac6e0e3c92023ccc2c6de49d22b411fc8b9e6dd03e7576acc1bb5870a6951181129ba77f3bf94bb45fe9c70105b1b896b9bb9
# tar -xzvf dotnet-sdk-8.0.404-linux-x64.tar.gz
...
```

#### vscode.sh
```sh

```

### Create the menu entry
```sh
cat <<SCRIPT > ~/.local/share/applications/vscode.sh
PATH="$HOME/BIN/dotnet8:$PATH"
code
SCRIPT

chmod +x ~/.local/share/applications/vscode.sh

cat <<SHORTCUT > ~/.local/share/applications/vscode.desktop 
#!/usr/bin/env xdg-open
[Desktop Entry]
Version=1.0
Name=My Vscode
Comment=My Vscode
Exec=$HOME/.local/share/applications/vscode.sh
Icon=code
Terminal=false
Type=Application
Categories=Development;IDE;
SHORTCUT

chmod +x ~/.local/share/applications/vscode.desktop 

update-desktop-database ~/.local/share/applications/
```