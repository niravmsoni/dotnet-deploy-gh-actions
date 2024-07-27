# Configuring Dependabot
In this post, we'll go over how can we automate Project dependency updates for .NET Applications

## Problem statement
In the fast-paced world of software development, maintaining up-to-date dependencies is crucial for ensuring security, performance, and functionality. There's a common saying: "Don't reinvent the wheel." When building .NET applications, our primary focus as developers should be on solving the core problems at hand. Often, for common challenges, there are already existing solutions in the form of packages on [NuGet](https://www.nuget.org/). All we need to do is pick the appropriate version and integrating it into our solution. This can significantly accelerate our development.

However, each coin has 2 sides. As soon as we decide to integrate any existing packages within our solution, we're in a way adding Technical Debt to our solution.
In my experience, after adding dependencies, most of the developers do not regularly check for updated versions of their dependencies. 
The dependencies (Specifically versions) are often looked at when:
- Upgrading the framework version (e.g., from .NET Core 6 to .NET Core 8) forces a dependency update.
- A [NuGet scan step](https://learn.microsoft.com/en-us/nuget/concepts/auditing-packages) in your CI pipeline reports a vulnerability.
In these cases, developers reactively review and update their dependencies.

Is there a better way to handle this problem proactively?

## Enter Dependabot
- [Dependabot](https://docs.github.com/en/code-security/dependabot/working-with-dependabot) is a powerful tool integrated with GitHub and helps us update the dependency of our solutions
- For enabling Dependabot support for your repository, go to Settings tab and you'll find a section namely "Code Security and analysis" where you'll find Dependabot related configurations

![1](https://github.com/user-attachments/assets/a7df828a-5594-41ba-aadf-a38ba3a1c380)
- Let's look at what this individual options do:
  - Enable `Dependabot alerts` - For receiving notifications regarding dependabot either in GitHub   notifications or over Emails
  - Enable `Dependabot security updates` - Using this, Dependabot can find out vulnerabilities which are exposed and it can alert us
  - Configure `Dependabot version updates` - Using this, we can enable the version updates for dependencies (For example - A new version for `coverlet.collector` is available on NuGet `(6.0.2)` and our code is using an older version `(6.0.0)`, then we can use option to enable version updates

## Configure Dependabot version updates
- Upon clicking Configure Dependebot version updates, it will create a setting which is configured as GitHub action namely `dependabot.yml` under .github folder
![2](https://github.com/user-attachments/assets/c0482881-5795-4495-85e3-8c306dbecbb4)
- Let's understand these options:
  - `package-ecosystem` - Since we're dealing with a .NET project, we can enter `nuget` here
  - `directory` - Since we want Dependabot to scan the entire repository, we'll leave this as `/`
  - `schedule` - For the sake of demo, let's go with `daily`
 
- There are other tons of other configuration options available which you can check out [here](https://docs.github.com/code-security/dependabot/dependabot-version-updates/configuration-options-for-the-dependabot.yml-file)

## Dependabot in-action
- Based on the above configurations, a bot would keep scan the repository and generate pull requests by updating the dependency version
- ![image](https://github.com/user-attachments/assets/b0d515d7-fddd-469e-9c98-fa765cb671ae)
- As you can see, it immediately created 5 PRs (pull requests) within my repository since there are newer package versions available for these dependencies
- Let's examine one of them in-detail
- ![image](https://github.com/user-attachments/assets/94d806f2-70f2-4dad-b29f-9de7436b21f3)
- ![image](https://github.com/user-attachments/assets/07690b0b-fb71-486c-a70d-f6c8f4198283)
- Dependabot provides a clear and concise summary in the description by providing us the following information
  - Release notes
  - Commits
  - Compatability
- Within the commits section, we can see that it has infact bumped up the version of `coverlet.coverage` package from `6.0.0` to `6.0.2` within the .csproj file.
- We can then review the details attached within the PR and act upon it.

## Caution - Test before you merge!
- One should keep in mind that this automated process is just to help us eliminate the manual task of updating the packages manually.
- One should carefully review the Release notes and upon confirming there are no breaking changes, one should test the functionalities before merging them into the main branch

## Advantages of Dependabots
  - Automated PRs
    - Dependabot modifies our code and raises PRs automatically for the updates to the dependencies and we can act upon them
  - Automated Security updates
    - Dependabot identifies vulnerabilities and if there are potential fixes identified for the same, it'll raise a PR for the same as well
  - Enhanced Security
    - Outdated dependencies often harbor vulnerabilities that can be exploited by attackers. Dependabot automatically checks for security updates and creates pull requests to address these issues, helping to keep your project secure with minimal manual intervention.
  - Improved efficiency
    - Manual dependency management is time-consuming and prone to human error. Dependabot automates the process, saving developers time and reducing the risk of introducing bugs. This allows teams to focus more on core development tasks rather than maintenance.
  - Consistent Updates
    - Regularly updating dependencies ensures that your project benefits from the latest features, performance improvements, and bug fixes. Dependabot continuously monitors your dependencies and provides updates, ensuring your project remains current and reliable.
      
## Summary and References
Automated dependency updates with Dependabot provide a proactive approach to dependency management, enhancing security, efficiency, and consistency in your projects. By leveraging Dependabot, development teams can maintain a healthy codebase with minimal effort, allowing them to focus on delivering high-quality software.

If you're using any other VCS like [Azure DevOps](https://azure.microsoft.com/en-in/products/devops/), this is not baked into it natively. However, there are 3rd party tools like [this](https://github.com/tinglesoftware/dependabot-azure-devops) available which can be used to integrate Dependabot into Azure DevOps.
Microsoft has announed that they're officially planning to bring in Dependabot security updates within Azure DevOps soon. See [here](https://learn.microsoft.com/en-us/azure/devops/release-notes/roadmap/2024/ghazdo/dependabot)

- [Dependabot for Azure DevOps](https://github.com/tinglesoftware/dependabot-azure-devops)
- [Configuring Dependabots](https://docs.github.com/en/code-security/dependabot/dependabot-version-updates/configuration-options-for-the-dependabot.yml-file)
- [Microsoft Learn Module for Dependabots](https://learn.microsoft.com/en-us/training/modules/configure-dependabot-security-updates-on-github-repo/)
