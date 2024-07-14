# CI/CD using Github Actions
Recently, I've been interacting with a lot of open-source repositories on GitHub and most of them have automated builds setup with a built-in feature of GitHub namely [GitHub Actions](https://docs.github.com/en/actions)

Let's explore how we can setup a full-fledged a CI/CD pipeline using GitHub Actions. We'll be creating a sample WebAPI and deploying it to Azure App Service

## Getting started with GitHub Actions
GitHub Actions is a CI/CD platform that allows you to automate your build, test, and deployment pipeline. You can create workflows that build and test every pull request to your repository or deploy merged pull requests to production.

GitHub Actions goes beyond just DevOps and lets you run workflows when other events happen in your repository. For example, you can run a workflow to automatically add the appropriate labels whenever someone creates a new issue in your repository.

GitHub provides Linux, Windows, and macOS virtual machines to run your workflows, or you can host your own self-hosted runners in your own data center or cloud infrastructure.

## Components of GitHub Actions
There are several components to a GitHub Actions namely:
   - Workflows
   - Events
   - Jobs
   - Actions
   - Runners

Let's deep dive into Workflows.

## What are Workflows?
A workflow is a configurable automated process that will run one or more jobs. Workflows are defined by a YAML file checked in to your repository and will run when triggered by an event in your repository, or they can be triggered manually, or at a defined schedule.

Workflows are defined in the `.github/workflows` directory in a repository, and a repository can have multiple workflows, each of which can perform a different set of tasks. For example, you can have one workflow to build and test pull requests, another workflow to deploy your application every time a release is created, and still another workflow that adds a label every time someone opens a new issue.

You can reference a workflow within another workflow.

## Creating our first Workflow
I've used a boilerplate .NET Core WebAPI 8 template and pushed the code to [this](https://github.com/niravmsoni/dotnet-deploy-gh-actions/tree/main) repository.

### Pre-requisites
 - Azure WebApp
![1](https://github.com/user-attachments/assets/ec46bb2b-f885-480d-b3ea-333b5b583e7c)
 - Setup Environment Variables
![3](https://github.com/user-attachments/assets/4cf87166-042c-445f-9eb1-9a5ee5061a67)
 - Enable SCM Basic Auth Publishing Credentials - This will enable us to be able to publish our .zip from Github Actions
![3_1](https://github.com/user-attachments/assets/4bad3d81-ea26-4fea-a655-c9c583aa6459)
 - Store Publish Profile in Github Repository Secrets - Since publish profile is considered as a sensitive information, it is ideal to store it in a secret and reference it within the pipeline later from here
![image](https://github.com/user-attachments/assets/88f68165-be62-467b-8450-bbe143cf1750)

Once these settings are done, we can proceed with creating our first workflow!

### Navigate to Action tab
Go to the Actions tab of the repository and click on `New Workflow`

![image](https://github.com/user-attachments/assets/41dc3a91-0bae-4f7b-af13-cdb1e776d426)

### Decide on using Built-in Workflows or Custom Workflow
There are a lot of built-in workflows available on the Github Actions. If they fit your need, you can use them. Since we're learning to build one from the scratch, I'll selected `Setup a workflow yourself`

![image](https://github.com/user-attachments/assets/7d4b37eb-c08c-4812-8c44-ff0b8cf83143)

### Define your workflow
#### Step#1 - Setup triggers and workflow name
Using `on`, we can specify when do we want this workflow to be triggered? For now, I've configured it to work in two ways:
   - `push`
      - This will trigger the workflow when the code is pushed to the main branch
   - `workflow_dispatch`
      - This will give us a manual way to trigger the pipeline from the GitHub Actions UI  
   
```yaml
name: Build 🚀

on:
  workflow_dispatch:
  push:
    branches:
    - main
```
#### Step#2 - Setup any environment variables
If you're required to use any environment variables in the pipeline, that can be declared next. Here, I'm declaring an environment variable namely .NET version and I'm setting it to be `8.0.x`

```yaml
# Setup environment variables
env:
  DOTNET_VERSION: 8.0.x
```
#### Step#3 - Build Job
Within the next steps of the pipeline, we've build steps. We're requesting our job to run on the `ubuntu-latest` .
Then, we need to define the steps required to be run as a part of this job.
We've used the below steps:
   - `actions/checkout@v3`
      - This is to ensure our code is checked out from the main branch 
   - `actions/setup-dotnet@v3`
      - This is to download the specified version of .NET (In our case 8.0.x) within the VM
   - `dotnet restore ./Starter.sln`
      - Restore the solution
   -  `Checking NuGet Vulnerabilities`
      - This is a DevSecOps approach which most of the enterprises follow these days to ensure they catch any vulnerabilities earlier in their development stage
      - Within this step, we're usng the `dotnet list package` to list the Vulnerabilities in the project files we're using and pushing it to a file namely `vulnerabilities.txt`
      - If there are any vulnerabilities found, the pipeline would be blocked and no further steps would be executed
   - `dotnet build`
      - Building the projects.
      - `--configuration Release` switch will build the project with `release` configuration
      - `--no-restore` switch will instruct the compiler to build the project without restoring (Since we've explicitly restored the project earlier) and we can save time here
   - `dotnet test`
      - We would like to run the tests and along with that also publsh the code coverage report.
   - `Create Code Coverage`
      - Create code coverage report
   - `Publish Coverage report`
      - Publish the code coverage report as output of the Github summary
   - `Publish Artifact`
      - Using this step, the artifacts created in the build job will be exported so that we could import and use it in the next step 

```yaml
jobs:
  build:
    runs-on: ubuntu-latest
    
    steps:
      # Checkout code
      - uses: actions/checkout@v3

      - name: Setup .NET
        uses: actions/setup-dotnet@v3
        with:
          dotnet-version: ${{env.DOTNET_VERSION}}

      - name: Dotnet Restore
        run: dotnet restore ./Starter.sln

      - name: Checking NuGet vulnerabilites
        run: |
            dotnet list package --vulnerable --include-transitive  > vulnerabilities.txt
            # Check if vulnerabilities are found in vulnerabilities.txt
            if grep -q "Vulnerabilities found" vulnerabilities.txt; then
              echo "Vulnerabilities found. Blocking pipeline."
              exit 1
            else
              echo "No vulnerabilities found."
            fi

      # Specifying no-restore flag since we're already restoring in earlier.
      - name: Dotnet Build
        run: dotnet build ./Starter.sln --configuration Release --no-restore

      # Specifying no-restore and no-build to speed up the process
      - name: Dotnet Test
        run: dotnet test ./Starter.sln --configuration Release --no-restore --no-build --collect:"XPlat Code Coverage" --logger trx --results-directory coverage

      - name: Dotnet Publish
        run: dotnet publish ./Starter/Starter.WebAPI/Starter.WebAPI.csproj --configuration Release --no-build --output '${{ env.AZURE_WEBAPP_PUBLISH_PATH }}/myapp'
        
      - name: Code Coverage Summary Report
        uses: irongut/CodeCoverageSummary@v1.3.0
        with:
           filename: 'coverage/*/coverage.cobertura.xml'
           badge: true
           format: 'markdown'
           output: 'both'

      - name: Publish Code Coverage
        run: cat code-coverage-results.md >> $GITHUB_STEP_SUMMARY

      - name: Publish artifact
        uses: actions/upload-artifact@v4
        with:
          name: myapp
          path: .
    
```
#### Step#4 - Deploy Job
This is where we would be deploying the artifact generated in the previous step to Azure App Service. Web App and the App Service plan are already provisioned.
Within the `deploy` job, we're doing the following:

We've used the below steps:
   - `actions/download-artifact@v4`
      - This will download the artifact generated as the outcome of the build job
   - `azure/webapps-deploy@v2`
      - This is where we connect to Azure App Service and uploading the .zip file contents using the publish profile
      - Publish profile is generally considered as a secret so instead of hardcoding it here, it's better to reference it from Actions > Repository secrets
```yaml
deploy:
    runs-on: ubuntu-latest
    needs: build

    steps:
      - name: Download artifact from build job
        uses: actions/download-artifact@v4
        with:
          name: myapp
          path: .
          
      - name: Deploy to App Service
        uses: azure/webapps-deploy@v2
        with:
          app-name: ${{ env.AZURE_WEBAPP_NAME }}
          publish-profile: ${{ secrets.AZURE_PUBLISH_PROFILE }}
          package: "${{ env.AZURE_WEBAPP_PUBLISH_PATH }}/myapp"
```
### Executing the Workflow
As you can see both the build and the deploy jobs have been successfully completed
![image](https://github.com/user-attachments/assets/b2312ed6-8da5-47a4-82d9-c36ff58f04d7)

### Testing the application
After navigating to this URL, we're able to see our API deployed successfully
![2](https://github.com/user-attachments/assets/005472a8-fa0c-4bcf-83d0-8190ec83aa5a)
![4](https://github.com/user-attachments/assets/bbb94cc2-2bb6-4aeb-aa61-3f08d881986f)

## Give a Star! ⭐
Feel free to request an issue on github if you find bugs or request a new feature. Your valuable feedback is much appreciated to better improve this project. If you find this useful, please give it a star to show your support for this project.
