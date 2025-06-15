# Npm Features

npm (Node Package Manager) is the default package manager for Node.js, enabling you to install, update, and manage JavaScript packages. EasyKit integrates npm tools to streamline your workflow for JavaScript and Node.js projects.

## Introduction

npm allows you to manage project dependencies, run scripts, and automate builds. EasyKit provides a graphical interface for common npm operations, making it easier to manage packages and scripts without using the command line.

## Key Features

- **Download Node.js:** Opens the Node.js website for download/installation.
- **Check Node.js:** Checks Node.js installation status and PATH configuration (`node -v`, `npm -v`).
- **Configure npm Path:** Manually configure the npm path.
- **Diagnostics:** Troubleshoot npm issues.
- **Install:** Installs npm packages (`npm install`).
- **Update:** Updates packages using `npm update` or `ncu` (`npm-check-updates`).
- **Production Build:** Builds for production (`npm run build`).
- **Development Server:** Starts a development server (`npm run dev`).
- **Security Audit:** Runs a security audit (`npm audit`).
- **Custom Script:** Runs custom npm scripts (`npm run <script>`).
- **package.json Info:** Displays `package.json` information.
- **Reset Cache:** Resets the npm cache (`npm cache clean --force`).
- **Configure Path:** Configures the npm path.

## Usage

Access npm features in the "Features" section of EasyKit. You can also use the following commands in your terminal:

- **Install dependencies:**
  ```shell
  npm install
  ```
- **Update packages:**
  ```shell
  npm update
  ```
- **Run a script:**
  ```shell
  npm run <script>
  ```
- **Audit for vulnerabilities:**
  ```shell
  npm audit
  ```
- **Build for production:**
  ```shell
  npm run build
  ```

For more advanced usage, see the [official npm documentation](https://docs.npmjs.com/).
