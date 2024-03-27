# security / sonarqube

- tools: Herramienta que permite exportar a un archivo CSV los problemas identificados por el SonarQube en los proyectos. Se utiliza el API del SonarQube para realizar la extracción de información. Se generan dos archivos, el Primary-Report con los Bugs, Vulnerabilities y HotSpots; y el Secondary-Report con los Code Smells. En el Main del programa se debe parametrizar la URL, el usuario y la contraseña y debe estar en ejecución el SonarQube.

- docker-compose.yml: permite ejecutar sonarqube en docker compose.

- script: para habilitar sonarqube en una Azure SQL Database y en un Azure Container Instance.

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
