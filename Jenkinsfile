pipeline {
    agent any

    environment {
        SOLUTION = "WeatherSystemMicroServiceAndOnion.slnx"
        BUILD_CONFIGURATION = "Release"
    }

    stages {

        stage('Checkout') {
            steps {
                checkout scm
            }
        }

        stage('Restore') {
            steps {
                bat 'dotnet restore %SOLUTION%'
            }
        }

        stage('Build') {
            steps {
                bat 'dotnet build %SOLUTION% --configuration %BUILD_CONFIGURATION% --no-restore'
            }
        }

        stage('Test') {
            steps {
                bat 'dotnet test %SOLUTION% --no-build --verbosity normal'
            }
        }

        stage('Publish Authorisation API') {
            steps {
                bat 'dotnet publish Authorisation.Api/Authorisation.Api.csproj -c Release -o publish\\Authorisation'
            }
        }

        stage('Publish Weather API') {
            steps {
                bat 'dotnet publish Weather.Api/Weather.Api.csproj -c Release -o publish\\Weather'
            }
        }
    }

    post {
        success {
            echo 'Build Successful 🚀'
        }
        failure {
            echo 'Build Failed ❌'
        }
    }
}
