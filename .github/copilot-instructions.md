---
mode: "agent"
applyTo: "**"
---

1. Use Windows as the development environment, ensuring all terminal commands are PowerShell-compatible (e.g., use '&&' for command chaining).

2. Do not directly generate project architecture or settings; instead, provide PowerShell commands to establish the project structure.

3. Annotate all generated code at the function level.

4. For project planning, deliver a comprehensive analysis and framework recommendations, requiring explicit user confirmation before proceeding with the selected framework.

5. In coordinator (boom) mode, log all completed tasks in a timestamped, categorized task report file (report.md).

6. After the architecture phase, generate a specification file (spec.md) and a task list (todolist.md); the specification file must include flowcharts, sequence diagrams, association diagrams, and all relevant UML diagrams using Markdown-compatible diagram syntax (e.g., Mermaid) for full compatibility with Visual Studio Code.

7. Ensure all outputs are optimized for seamless integration and readability within Visual Studio Code's editor, output pane, and integrated terminal.

8. Guarantee that all generated content—including code, documentation, and diagrams—adheres to best practices for clarity, maintainability, and accessibility in Visual Studio Code.

9. Provide concise, actionable instructions for each step, and verify that all outputs are formatted for optimal display in the editor, output pane, and integrated terminal.

10. Optimize API Requests: When performing API requests, always utilize the API's batch processing capabilities to consolidate multiple operations into a single request whenever possible. This approach minimizes the number of API calls, reduces network overhead, and helps prevent exceeding rate limits. For example, if the API supports JSON batching, structure your requests accordingly to combine multiple operations into one HTTP call.
