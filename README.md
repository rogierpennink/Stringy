# Stringy

Stringy is a simple and light-weight runtime string interpolation engine. Originally built as an utility for a text-based game, the purpose of the library is to provide something that's easier to use and more light-weight than Runtime Text Templates or RazorEngine, but a little more powerful than `string.Replace()`.

## Use cases

Stringy favours light string replacement and basic logic and branching needs. If you want to produce large bodies of (formatted) text using complex logic, you will probably find it worth it to use a full templating solution like RazorEngine. Use Stringy if:

* Your string templates require basic logic and branching but you don't need the full power of C#.

* You keep strings in a database/persistent storage and your substitution needs are greater than what can be achieved with regular expressions and `string.Replace()`.

* You need to execute LINQ and/or custom methods on your model objects from your string templates.

## Installation

Simply add the Stringy NuGet package to your solution:

```powershell
dotnet add package Stringy
dotnet restore
```

Or, using powershell:

```powershell
Install-Package Stringy
```

## Examples

#### Simple string substitution

```C#
string str = "Hello World";
const string template = "Substitute this: {value}";

IStringy engine = new Stringy();
engine.Set("value", str);

// Prints: Substitute this: Hello World
Console.WriteLine(engine.Execute(template));
```

Using `engine.Set()` you can add any object and its methods and properties will be available in your string templates.

#### Executing basic math

```C#
int num1 = 6;
int num2 = 7;
const string template = "{a} + {b} = {a + b}";

IStringy engine = new Stringy();
engine.Set("a", num1);
engine.Set("b", num2);

// Prints: 6 + 7 = 42
Console.WriteLine(engine.Execute(template));
```

#### Calling methods and properties of objects

```C#
string str = "Hello World";
const string template = "The lowercase string '{value.ToLower()}' has {value.Length} characters";

IStringy engine = new Stringy();
engine.Set("value", str);

// Prints: The lowercase string 'hello world' has 11 characters
Console.WriteLine(engine.Execute(template));
```

#### Simple if/else statements

```C#
bool userIsFemale = true;
const string template = "Welcome, dear {isFemale ? 'Lady' : 'Sir'}";

IStringy engine = new Stringy();
engine.Set("isFemale", userIsFemale);

// Prints: Welcome, dear Lady
Console.WriteLine(engine.Execute(template));
```

## License

Apache 2.0