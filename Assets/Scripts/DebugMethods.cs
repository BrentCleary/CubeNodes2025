using System.Collections;
using System.Collections.Generic;
using Diag = System.Diagnostics;
using UnityEngine;

public class Example : MonoBehaviour
{
    void Start()
    {
        // LogCurrentLine();
    }

    void LogCurrentLine()
    {
        Diag.StackTrace stackTrace = new Diag.StackTrace(true);
        Diag.StackFrame frame = stackTrace.GetFrame(1); // Current method frame - Display name and line number of method containing LogCurrentLine()

        string fileName = frame.GetFileName(); // Gets file name
        int lineNumber = frame.GetFileLineNumber(); // Gets line number

        UnityEngine.Debug.Log($"Current line: {lineNumber} in file: {fileName}");
    }

    /* ChatGPT Conversation for reference: 11/01/24 - https://chatgpt.com/c/672592ee-f754-8008-839c-90212ae0b9f3

    Is there a function to set a perpetual reference to a methods line in a document even if it is not throwing an error.

Error codes display line numbers. Can I get a debugMessage to display the current line number of a working method dynamically

ChatGPT said:
ChatGPT
Yes, you can display the current line number in a Debug.Log message by using System.Diagnostics.StackTrace. This allows you to retrieve information about the call stack, including the file name and line number, even in non-error scenarios.

Here's an example of how to use it in Unity:

csharp
Copy code
using System.Diagnostics;
using UnityEngine;

public class Example : MonoBehaviour
{
    void Start()
    {
        LogCurrentLine();
    }

    void LogCurrentLine()
    {
        StackTrace stackTrace = new StackTrace(true);
        StackFrame frame = stackTrace.GetFrame(0); // Current method frame

        string fileName = frame.GetFileName(); // Gets file name
        int lineNumber = frame.GetFileLineNumber(); // Gets line number

        Debug.Log($"Current line: {lineNumber} in file: {fileName}");
    }
}
Explanation
StackTrace captures information about the call stack, with each StackFrame representing a specific call (method invocation).
GetFrame(0) retrieves the current frame, where LogCurrentLine() was called.
GetFileLineNumber() and GetFileName() return the line number and file name where the call occurred.
Important Notes
Performance: Using StackTrace can be slow, especially if called frequently.
Editor-Only: Line numbers and file names are generally available only in the Unity Editor with debugging information enabled. In production builds, this data might be unavailable or stripped out.


You said:
There is ambiguity between UnityEngine and SystemDiagnostics.
ChatGPT said:
ChatGPT
In cases where you have naming conflicts between classes in different namespaces (like UnityEngine.Debug and System.Diagnostics.Debug), you can resolve the ambiguity by specifying the full namespace for the type you're using. Here’s how to apply this in your code:

Explicitly specify the namespace for each conflicting usage.
Alias one of the namespaces for easier reference.
Example with Full Namespace Specification
If you only need to call Debug.Log from Unity and StackTrace from System.Diagnostics, specify them fully:

csharp
Copy code
using UnityEngine;
using System.Diagnostics;

public class Example : MonoBehaviour
{
    void Start()
    {
        LogCurrentLine();
    }

    void LogCurrentLine()
    {
        System.Diagnostics.StackTrace stackTrace = new System.Diagnostics.StackTrace(true);
        System.Diagnostics.StackFrame frame = stackTrace.GetFrame(0); // Current method frame

        string fileName = frame.GetFileName(); // Gets file name
        int lineNumber = frame.GetFileLineNumber(); // Gets line number

        UnityEngine.Debug.Log($"Current line: {lineNumber} in file: {fileName}");
    }
}
Example with Namespace Aliasing
Alternatively, you can set an alias for System.Diagnostics to make it easier to call members within it:

csharp
Copy code
using UnityEngine;
using Diag = System.Diagnostics;

public class Example : MonoBehaviour
{
    void Start()
    {
        LogCurrentLine();
    }

    void LogCurrentLine()
    {
        Diag.StackTrace stackTrace = new Diag.StackTrace(true);
        Diag.StackFrame frame = stackTrace.GetFrame(0); // Current method frame

        string fileName = frame.GetFileName(); // Gets file name
        int lineNumber = frame.GetFileLineNumber(); // Gets line number

        Debug.Log($"Current line: {lineNumber} in file: {fileName}");
    }
}
With either approach, the ambiguity is resolved while retaining access to both UnityEngine.Debug and System.Diagnostics.


You said:
So call this method in another method, and it will tell me the line and name of the method containing it?
ChatGPT said:
ChatGPT
Yes, calling LogCurrentLine() from within another method will provide you with the line number and method name of the calling method. However, to capture the caller's method name and line number, you need to adjust the StackFrame index to 1 instead of 0 (since 0 would reference LogCurrentLine() itself). Here's how you can do it:

csharp
Copy code
using UnityEngine;
using System.Diagnostics;

public class Example : MonoBehaviour
{
    void Start()
    {
        TestMethod();
    }

    void TestMethod()
    {
        LogCurrentLine(); // Call within this method to log its line and method name
    }

    void LogCurrentLine()
    {
        StackTrace stackTrace = new StackTrace(true);
        StackFrame frame = stackTrace.GetFrame(1); // Get the frame of the calling method

        string methodName = frame.GetMethod().Name; // Gets the calling method name
        string fileName = frame.GetFileName();      // Gets file name
        int lineNumber = frame.GetFileLineNumber(); // Gets line number

        UnityEngine.Debug.Log($"Method: {methodName}, Line: {lineNumber} in file: {fileName}");
    }
}
Explanation
GetFrame(1): This captures the calling method’s frame, so LogCurrentLine() will output the line number and name of the method that called it (in this case, TestMethod).
frame.GetMethod().Name: This retrieves the name of the calling method.
Output Example
When TestMethod() calls LogCurrentLine(), you’ll see output like:

mathematica
Copy code
Method: TestMethod, Line: 15 in file: Assets/Scripts/Example.cs
This tells you the line in TestMethod where LogCurrentLine() was invoked, along with the file and method name.






You said:
Thank you. Very helpful.
ChatGPT said:
ChatGPT
You're very welcome! I'm glad it helped! 😊 Debugging with line numbers and method names can be a huge time-saver. If you have any more questions on debugging or anything else in Unity, feel free to ask. Happy coding! 🎮✨

    */

}
