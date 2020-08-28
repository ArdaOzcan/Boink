# Boink
![](https://raw.githubusercontent.com/ArdaOzcan/Boink/master/res/boink_logo.svg)

Boink is an imperative and interpreted language written in C# for educational purposes and is currently in development.

Aim of Boink is to create a language that is both type safe and easy to write. And also to learn how components of an interpreted language works.

## Information
---
### Framework & Language
- Boink is written with **.NET Core** and **C#**. This means Boink is cross-platform since .NET Core is also cross-platform.

- Boink also contains little **Python 3** scripts for automation of small things while development, these python scripts are not included in Boink binaries.
## Contributing
---
You can contribute by opening issues, participating to them or modifying the source code to improve Boink.
### Downloading the Source
#### 1. With git
**Note:** You must have git installed on your device to do this.

1. Create a suitable directory for Boink source and open a terminal in that directory.
2. Execute command `git clone https://github.com/ArdaOzcan/Boink.git`

#### 2. With GitHub
1. Go to the top and click the green button that says 'Code' and then click 'Download ZIP'.
### Dependencies
#### 1. .NET Core
You need to have .NET Core set up on your machine in order to develop Boink language. It is cross-platform so you can download it according to your operating system.

You can download it from [here](https://dotnet.microsoft.com/download).
#### 2. Python 3
Python 3 is not necessary but useful for development. Things like generating test cases are automated using python, so having it set up is nice.

You can download it from [here](https://www.python.org/downloads/).
### Setting Up the IDE & Workspace
#### Visual Studio Code
It is recommended to use Visual Studio Code since it has nice features and lots of third-party extensions. You can download it from [here](https://code.visualstudio.com/download).
##### Used Extensions:
1. C# **(ms-dotnettools.csharp)**
2. Python **(ms-python.python)**
3. C# Sort Using **(jongrant.csharpsortusings) (Optional)**
4. C# XML Documentation Comments **(k--kato.docomment) (Optional)**
5. Python Docstring Generator **(njpwerner.autodocstring) (Optional)**
6. Markdown Preview Mermaid Support **(bierner.markdown-mermaid) (Optional)**

After you've set up the extension you want, you can directly open the root folder **Boink/** in vscode.

Alternatively, you can set up a workspace by adding different source folders you want to your workspace. Source folders have their own README files for additional information on what they try to achieve.
#### Visual Studio
You can open the Boink.sln file in Visual Studio but a tutorial is not included here. _You might have to install .NET support to Visual Studio._
### Developing, Building & Testing
#### Changing or Adding to the Source Code
---
You can change the source code to improve Boink but you have to follow the guidelines.

- Follow the [dofactory style guide](https://www.dofactory.com/reference/csharp-coding-standards).

- You should modify or add XML comments to classes, methods, fields and properties. You can use the **C# XML Documentation Comments (k--kato.docomment)** extension if you use vscode.
#### Building
---
Go to the directory where .csproj file exists to build a single project, go to the root folder where the .sln file exists to build all of the projects and run `dotnet build`
#### Unit Testing
---
Go to `Boink/src/boinktest` and run `dotnet test` to see the results of the tests and debug accordingly.

## Features & Programming Approach of Boink
---
### Control Flow
#### Without Functions
A Boink program starts interpreting from the first line of the given file. It doesn't require any main function or a class to start executing. This means you can write a Boink program without any functions (Ignoring the fact that the program itself is a function).
```
# Interprets correctly
int b = 1 + 2
int a = b * 2
```
#### With Functions
You can alter the control flow using function calls as you would in languages like Javascript or Python.
```
fn executeAfter(int b)
    # Executed second.
    int c = b + 1
;

# Executed first.
int b = 4

executeAfter(b)
```

### Types
Boink is a type safe language aiming to prevent any type mismatch. This means every variable should be declared with a valid type. If not, an error will be thrown _before_ runtime.
#### Currently valid types:
- int
- float
- double
- bool
- string
- ~~dyn~~ (dyn is not implemented but still exists as a class)
#### Types planned to be added:
- list
- array
- set
#### Example:
```
# Correct.
int a
a = 1
```
```
# Throws an error.
int a
a = 1.5
```
```
# Correct.
# (Arrow operator signifies return type)
fn half(int whole) -> float
    give whole/2
;

float x = half(1)   # gives 0.5
```
```
fn half(int whole) -> double
    give whole/2
;

# Throws an error.
# int and double mismatch
int x = half(1)
```
### Definitions
Just like type checking, Boink also checks for previous definitions of variables and throws errors accordingly. A variable (including functions) must be defined before reference and only once.
#### Global Variables
Global variables in Boink are readable from inner scopes but can't be changed globally. Attempt to assign or declare a variable with the same name will override the definition of the global variable.
##### Example:
```
double PI = 3.1415
fn getPerimeter(double radius) -> double
    # Can perfectly read global variable PI.
    give PI * radius * 2
;

double perimeter = getPerimeter(2.0)  # gives 12.566
```
```
int foo = 1
fn bar() -> int
    foo = 2 # Override the 'foo' definition
    give foo + 1
;

int result = bar()  # It's 3 but 'foo' is still 1 in this scope
```
You can also override a global variable with a different type.
```
int foo = 1
fn bar() -> double
    double foo = 2.0 # Override the 'foo' definition with another type
    give foo + 1
;

float result = bar() # It's 3 but 'foo' is still 1 in this scope.
```
Same logic for overriding applies to nested functions.
```
fn foo() -> int
    int one = 0
    fn bar() -> int
        int one = 1 # Override 'one' from outer scope
        give one
    ;
    give bar()
;
int result = foo() # It's 1
```
Note that this overriding only works for different scopes. For example:
```
int b = 1
# Throws an error.
int b = 2
```
#### References
Every variable should be declared before being referenced. Even though they would be theoretically declared during runtime, semantic analyzer forces variables to be declared _literally_ before a reference (as in line number or position) since Boink uses a top-to-down approach when reading syntax nodes.
```
int a = 1
fn increment() -> int
    # Works.
    give a + 1
;

int b = increment() # Gives 2
```
```
fn increment() -> int
    # Semantic analyzer throws an error
    # because 'a' is not defined before this declaration
    # even though this would work in Python
    give a + 1
;

int a
int b = increment()
```
## Technique
### 1. Lexical Analysis
Boink first goes through every character and converts words into Tokens which is also called tokenization.
#### Example:
```
bool definitelyNotTrue = false
if(definitelyNotTrue)
    int numberOfTheBeasts = 666
;
```
*Lexed tokens in this example tokens would be as follows:*
- BoolType,
- Word,
- Equals,
- BoolLiteral,
- NewLine,
- If,
- LeftParenthesis
- Word,
- RightParenthesis,
- NewLine,
- IntType,
- Word,
- Equals,
- IntLiteral,
- SemiColon,
- NewLine _(A new line is always added to the text by Boink)_

### 2. Parsing
Every token is processed and converted into meaningful syntax nodes while parsing which make up the abstract syntax tree. An abstract syntax tree _(AST for short)_ allows Boink to traverse in it and make the Boink code easier to process.
#### Example:
```
fn thisIsAFunc(int argOne) -> int
    int result = argOne
    give result
;
```
Parsed tree would be as follows for this code:
```
{
    "ProgramSyntax": {
        "Statements": [
            {
                "FunctionSyntax": {
                    "Name": "<Word: 'thisIsAFunc'>",
                    "Arguments": [
                        {
                            "DeclarationSyntax": {
                                "Variable Type": {
                                    "TypeSyntax": {
                                        "Token": "<IntType: 'int'>"
                                    }
                                },
                                "Name": "argOne"
                            }
                        }
                    ],
                    "Statements": [
                        {
                            "DeclarationSyntax": {
                                "Variable Type": {
                                    "TypeSyntax": {
                                        "Token": "<IntType: 'int'>"
                                    }
                                },
                                "Name": "result",
                                "Expression": {
                                    "VariableSyntax": {
                                        "Name": "argOne"
                                    }
                                }
                            }
                        },
                        {
                            "GiveSyntax": {
                                "Expression": {
                                    "VariableSyntax": {
                                        "Name": "result"
                                    }
                                }
                            }
                        }
                    ]
                }
            }
        ]
    }
}
```
### 3. Semantic Analysis
Syntax nodes which were generated by the parser are now analyzed for [type](###types) and [definition](###definitions) checking. Semantic analyzer traverses through the AST and gives information or errors if necessary which will stop the interpreter from execution.
#### Example:
```
int a = b
```
##### Output:
```
------ Semantic Analysis ----

DEFINITION: Boink.Types.int_: a defined in scope global.

------------- End -----------
ERRORS:
UndefinedSymbolError: Variable 'b' is not defined. Error Position: (1, 9)
IncompatibleTypesError: Type  and Boink.Types.int_ are not compatible for assignment. Error Position: (1, 1)

```
### 4. Interpretation
Interpretation is the process where the code actually _runs_. Instead of compiling the source code (or the AST) into a binary file, Boink just converts them into C# code which is executed **_during the process of reading the AST_**. So Boink doesn't deal with any assembly or machine code, it just _interprets_ the Boink syntax into C# syntax just in time.

During interpretation, information about a function call and variables inside that function are stored in data structures called activation records. Activation records are stored in the call stack. These records actually store a value of a variable _(unlike the semantic analyzer)_ and can read/write to those variables.

#### Example:
```
fn foo() -> int
    int one = 0
    fn bar() -> int
        int one = 1 # Override 'one' from outer scope
        give one
    ;
    give bar()
;
int result = foo() # It's 1
```
##### Output:
```
------- START OF FUNCTION test.boink -------
------- START OF FUNCTION foo -------
------- START OF FUNCTION bar -------
-------- END OF FUNCTION bar --------
CALL STACK
3 bar
   one: 1

2 foo
   one: 0
   bar: <function_ 'bar'>

1 test.boink
   foo: <function_ 'foo'>



-------- END OF FUNCTION foo --------
CALL STACK
2 foo
   one: 0
   bar: <function_ 'bar'>

1 test.boink
   foo: <function_ 'foo'>



-------- END OF FUNCTION test.boink --------
CALL STACK
1 test.boink
   foo: <function_ 'foo'>
   result: 1
```
