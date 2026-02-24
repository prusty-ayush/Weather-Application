pipeline {
    agent any

    environment {
        DOTNET_ROOT = "/usr/bin/dotnet"
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
                sh 'dotnet restore $SOLUTION'
            }
        }

        stage('Build') {
            steps {
                sh 'dotnet build $SOLUTION --configuration $BUILD_CONFIGURATION --no-restore'
            }
        }

        stage('Test') {
            steps {
                sh 'dotnet test $SOLUTION --no-build --verbosity normal'
            }
        }

        stage('Publish Authorisation API') {
            steps {
                sh 'dotnet publish Authorisation.Api/Authorisation.Api.csproj -c Release -o publish/Authorisation'
            }
        }

        stage('Publish Weather API') {
            steps {
                sh 'dotnet publish Weather.Api/Weather.Api.csproj -c Release -o publish/Weather'
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