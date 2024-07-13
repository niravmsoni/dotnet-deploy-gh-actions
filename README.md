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

### Step#1 - Navigate to Action tab
Go to the Actions tab of the repository and click on `New Workflow`

![image](https://github.com/user-attachments/assets/41dc3a91-0bae-4f7b-af13-cdb1e776d426)

### Step#2 - Decide on using Built-in Workflows or Custom Workflow
There are a lot of built-in workflows available on the Github Actions. If they fit your need, you can use them. Since we're learning to build one from the scratch, I'll selected `Setup a workflow yourself`

![image](https://github.com/user-attachments/assets/7d4b37eb-c08c-4812-8c44-ff0b8cf83143)

### Step#3 - Build your workflow


