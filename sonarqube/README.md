# security / sonarqube

- tools: Herramienta que permite exportar a un archivo CSV los problemas identificados por el SonarQube en los proyectos. Se utiliza el API del SonarQube para realizar la extracción de información. Se generan dos archivos, el Primary-Report con los Bugs, Vulnerabilities y HotSpots; y el Secondary-Report con los Code Smells. En el Main del programa se debe parametrizar la URL, el usuario y la contraseña y debe estar en ejecución el SonarQube.

- docker-compose.yml: permite ejecutar sonarqube en docker compose.

- script: para habilitar sonarqube en una Azure SQL Database y en un Azure Container Instance.

## Deploy Azure SQL Database y Azure Container Instance

```powershell
# Create Resource Group
az group create --name "lcs16-sonarqube-rg" --location "eastus"
# Create Azure SQL Server
az sql server create --resource-group "lcs16-sonarqube-rg" --name "lcs16-sql-sonarqube" --location "eastus" --admin-user "sonarqube" --admin-password "W1fwU7R4XFLoLhY4PHw80jmhN"
# Configure Azure SQL Server firewall
az sql server firewall-rule create --resource-group "lcs16-sonarqube-rg" --server "lcs16-sql-sonarqube" --name "AllIpAddress" --start-ip-address 0.0.0.0 --end-ip-address 255.255.255.255
# Create Azure SQL Database
az sql db create --resource-group "lcs16-sonarqube-rg" --server "lcs16-sql-sonarqube" --name "sonarqube" --edition GeneralPurpose --family Gen5 --capacity 1 --compute-model Serverless --max-size 10GB --auto-pause-delay 60 --collation "SQL_Latin1_General_CP1_CS_AS"
# Create Azure Container Instance
az container create --resource-group "lcs16-sonarqube-rg" --name "lcs16-container-sonarqube" --dns-name-label "lcs16-container-sonarqube" --location "eastus" --image "sonarqube:10.4-community" --cpu 1 --memory 2 --os-type Linux --ip-address Public --ports 9000 --protocol TCP --environment-variables SONAR_ES_BOOTSTRAP_CHECKS_DISABLE="true" SONAR_JDBC_USERNAME="sonarqube" SONAR_JDBC_PASSWORD="W1fwU7R4XFLoLhY4PHw80jmhN" SONAR_JDBC_URL="jdbc:sqlserver://lcs16-sql-sonarqube.database.windows.net:1433;database=sonarqube;user=sonarqube@lcs16-sql-sonarqube;password=W1fwU7R4XFLoLhY4PHw80jmhN;encrypt=true;trustServerCertificate=false;hostNameInCertificate=*.database.windows.net;loginTimeout=30;"
```

## Deploy Azure Virtual Machine

```powershell
"n" | ssh-keygen -t rsa -b 4096 -C "azureadministrator" -f "$ENV:UserProfile/.ssh/lcs16-vm-sonar" -P "azureprueba123*"
az vm create --name "lcs16-vm-sonar" --resource-group "lcs16-rg" --location "eastus" --image "Canonical:0001-com-ubuntu-server-jammy:22_04-lts-gen2:latest" --size "Standard_B2ms" --admin-username "azureadministrator" --ssh-key-values "~/.ssh/lcs16-vm-sonar.pub" --os-disk-size-gb 32 --public-ip-sku "Standard" --public-ip-address-dns-name "lcs16-vm-sonar"

# habilitar auto-shutdown
az vm auto-shutdown --name "lcs16-vm-sonar" --resource-group "lcs16-rg" --time 0000

# habilitar puerto
az vm open-port --name "lcs16-vm-sonar" --resource-group "lcs16-rg" --port 9000 --priority 100

# conectar ssh
ssh -i ~/.ssh/lcs16-vm-sonar "azureadministrator@lcs16-vm-sonar.eastus.cloudapp.azure.com"

# instalar docker
sudo su
curl -fsSL https://get.docker.com | sudo sh

# descargar docker-compose.yml
curl -fsSL https://raw.githubusercontent.com/luiscasalas16/notes-security/main/sonarqube/docker-compose.yml -o docker-compose.yml

# ejecuar docker-compose.yml
docker compose up -d
```
