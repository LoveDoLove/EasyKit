# Git Features

Git is a distributed version control system that helps you track changes, collaborate with others, and manage your project's history. EasyKit integrates Git operations into a user-friendly interface, making source control accessible for all users.

## Introduction

With Git, you can manage your codebase, experiment with new features, and collaborate with team members efficiently. EasyKit provides a graphical interface for common Git commands, reducing the need to use the command line for everyday tasks.

## Key Features

- **Init:** Initializes a new Git repository (`git init`).
- **Status:** Shows repository status, including modified, staged, and untracked files (`git status`).
- **Add:** Adds changes to the staging area (`git add .` or `git add <file>`).
- **Commit:** Commits staged changes with a message (`git commit -m "message"`).
- **Push:** Pushes changes to a remote repository (`git push`).
- **Pull:** Pulls changes from a remote repository (`git pull`).
- **Create Branch:** Creates a new branch (`git branch <branch>` or `git switch -c <branch>`).
- **Switch Branch:** Switches to an existing branch (`git switch <branch>`).
- **Merge:** Merges a branch into the current branch (`git merge <branch>`).
- **History:** Displays commit history (`git log`).
- **Stash:** Temporarily saves changes (`git stash`).
- **Apply Stash:** Applies stashed changes (`git stash apply`).
- **Create Pull Request (Info):** Information on creating pull requests.
- **List Pull Requests (Info):** Information on listing pull requests.
- **Diagnostics:** Checks Git installation and repository status.
- **Add Submodule:** Adds a new submodule (`git submodule add <repo>`).
- **Update Submodule:** Updates submodules (`git submodule update --remote`).
- **Remove Submodule:** Removes a submodule.

## Usage

Access the Git menu via "Git Tools" in the main menu. Select the desired operation, and EasyKit will prompt for any required information. You can also use the following commands directly in a terminal:

- **Check status:**
  ```shell
  git status
  ```
- **View commit history:**
  ```shell
  git log
  ```
- **Show changes:**
  ```shell
  git diff
  ```
- **List branches:**
  ```shell
  git branch
  ```
- **Switch branches:**
  ```shell
  git switch <branch>
  ```
- **Merge branches:**
  ```shell
  git merge <branch>
  ```

For more advanced workflows, refer to the [official Git documentation](https://git-scm.com/doc).