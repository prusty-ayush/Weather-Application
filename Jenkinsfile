// pipeline {
//     agent any

//     environment {
//         SOLUTION = "WeatherSystemMicroServiceAndOnion\\WeatherSystemMicroServiceAndOnion.slnx"
//         BUILD_CONFIGURATION = "Release"
//     }

//     stages {

//         stage('Checkout') {
//             steps {
//                 checkout scm
//             }
//         }

//         stage('Debug Workspace') {
//             steps {
//                 bat "echo Current Directory:"
//                 bat "cd"
//                 bat "echo Files in Workspace:"
//                 bat "dir"
//             }
//         }

//         stage('Restore') {
//             steps {
//                 bat "dotnet restore %SOLUTION%"
//             }
//         }

//         stage('Build') {
//             steps {
//                 bat "dotnet build %SOLUTION% --configuration %BUILD_CONFIGURATION% --no-restore"
//             }
//         }

//         stage('Test') {
//             steps {
//                 bat "dotnet test %SOLUTION% --no-build --verbosity normal"
//             }
//         }

//         stage('Publish Authorisation API') {
//             steps {
//                 bat "dotnet publish WeatherSystemMicroServiceAndOnion\\Authorisation.Api\\Authorisation.Api.csproj -c Release -o publish\\Authorisation"
//             }
//         }

//         stage('Publish Weather API') {
//             steps {
//                 bat "dotnet publish WeatherSystemMicroServiceAndOnion\\Weather.Api\\Weather.Api.csproj -c Release -o publish\\Weather"
//             }
//         }
//     }

//     post {
//         success {
//             echo 'Build Successful 🚀'
//         }
//         failure {
//             echo 'Build Failed ❌'
//         }
//     }
// }



pipeline {
    agent any

    tools {
        dotnetsdk 'Default'
    }

    stages {

        stage('Checkout') {
            steps {
                checkout scm
            }
        }

        stage('SonarQube Analysis') {
            steps {
                script {
                    def scannerHome = tool 'SonarScanner for .NET'
                    withSonarQubeEnv('SonarQube') {

                        bat """
                        "${scannerHome}\\SonarScanner.MSBuild.exe" begin ^
                        /k:"weather-app" ^
                        /d:sonar.host.url="http://localhost:9000"

                        dotnet restore WeatherSystemMicroServiceAndOnion.slnx
                        dotnet build WeatherSystemMicroServiceAndOnion.slnx --no-restore

                        "${scannerHome}\\SonarScanner.MSBuild.exe" end
                        """
                    }
                }
            }
        }

        stage("Quality Gate") {
            steps {
                timeout(time: 5, unit: 'MINUTES') {
                    waitForQualityGate abortPipeline: true
                }
            }
        }
    }

    post {
        success {
            echo 'Sonar Analysis Successful ✅'
        }
        failure {
            echo 'Build Failed ❌'
        }
    }
}
