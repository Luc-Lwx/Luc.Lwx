

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






