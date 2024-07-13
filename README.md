# dotnet-deploy-gh-actions
Recently, I've been interacting with a lot of open-source repositories on GitHub and most of them have automated builds setup with a built-in feature of GitHub namely [GitHub Actions](https://docs.github.com/en/actions)

I've interacted a lot with the Azure DevOps in the past for setting up the CI/CD pipelines in the past, but I thought to explore and create a build pipeline using GitHub Actions

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

### Navigate to Action tab
Go to the Actions tab of the repository and click on `New Workflow`

![image](https://github.com/user-attachments/assets/41dc3a91-0bae-4f7b-af13-cdb1e776d426)

### Decide on using Built-in Workflows or Custom Workflow
There are a lot of built-in workflows available on the Github Actions. If they fit your need, you can use them. Since we're learning to build one from the scratch, I'll selected `Setup a workflow yourself`

![image](https://github.com/user-attachments/assets/7d4b37eb-c08c-4812-8c44-ff0b8cf83143)

### Build your workflow
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
#### Step#3 - Define Build Job
Within the next steps of the pipeline, we've specified a single job namely `build`. We're requesting our job to run on the `ubuntu-latest` .
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
      - Within this step, we're usng the ` dotnet list package` to list the Vulnerabilities in the project files we're using and pushing it to a file namely `vulnerabilities.txt`
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

```yaml
jobs:
  build:
    runs-on: ubuntu-latest
    
    steps:
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
            if grep -q "Vulnerabilities found" vulnerabilities.txt; then
              echo "Vulnerabilities found. Blocking pipeline."
              exit 1
            else
              echo "No vulnerabilities found."
            fi

      - name: Dotnet Build
        run: dotnet build ./Starter.sln --configuration Release --no-restore

      - name: Dotnet Test
        run: dotnet test ./Starter.sln --configuration Release --no-restore --no-build --collect:"XPlat Code Coverage" --logger trx --results-directory coverage

      - name: Code Coverage Summary Report
        uses: irongut/CodeCoverageSummary@v1.3.0
        with:
           filename: 'coverage/*/coverage.cobertura.xml'
           badge: true
           format: 'markdown'
           output: 'both'

      - name: Publish Code Coverage
        run: cat code-coverage-results.md >> $GITHUB_STEP_SUMMARY
    
```
