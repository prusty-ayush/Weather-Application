pipeline {
    agent any

    environment {
        SOLUTION = "WeatherSystemMicroServiceAndOnion\\WeatherSystemMicroServiceAndOnion.slnx"
        BUILD_CONFIGURATION = "Release"
    }

    stages {

        stage('Checkout') {
            steps {
                checkout scm
            }
        }

        stage('SonarQube Analysis + Build') {
            steps {
                script {
                    def scannerHome = tool 'SonarScanner for .NET'

                    withSonarQubeEnv('SonarQube') {

                        bat """
                        "${scannerHome}\\SonarScanner.MSBuild.exe" begin ^
                        /k:"weather-app" ^
                        /d:sonar.host.url="http://localhost:9000" ^
                        /d:sonar.login="%SONAR_AUTH_TOKEN%"

                        dotnet restore %SOLUTION%
                        dotnet build %SOLUTION% --configuration %BUILD_CONFIGURATION% --no-restore
                        dotnet test %SOLUTION% --no-build --verbosity normal

                        "${scannerHome}\\SonarScanner.MSBuild.exe" end ^
                        /d:sonar.login="%SONAR_AUTH_TOKEN%"
                        """
                    }
                }
            }
        }

        stage('Publish Authorisation API') {
            steps {
                bat "dotnet publish WeatherSystemMicroServiceAndOnion\\Authorisation.Api\\Authorisation.Api.csproj -c Release -o publish\\Authorisation"
            }
        }

        stage('Publish Weather API') {
            steps {
                bat "dotnet publish WeatherSystemMicroServiceAndOnion\\Weather.Api\\Weather.Api.csproj -c Release -o publish\\Weather"
            }
        }
    }

    post {
        success {
            echo 'Build + Sonar Successful 🚀'
        }
        failure {
            echo 'Build Failed ❌'
        }
    }
}
