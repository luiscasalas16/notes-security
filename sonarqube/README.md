# tools-sonarqube

Herramienta que permite exportar a un archivo CSV los problemas identificados por el SonarQube en los proyectos. Se utiliza el API del SonarQube para realizar la extracción de información. 

Se generan dos archivos, el Primary-Report con los Bugs, Vulnerabilities y HotSpots; y el Secondary-Report con los Code Smells. 

En el Main del programa se debe parametrizar la URL, el usuario y la contraseña y debe estar en ejecución el SonarQube.
