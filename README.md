# Mince
Mince is an interpreted programming language made in a month because I had nothing else to do.
It is fully functional and supports:
* Classes
* Primitive types (number, bool, string, etc...)
* Advanced types (Socket, File, etc...)
* Functions
* If, else if and else statements
* While loops, until loops, for loops and for each loops
* Dynamic object (a special kind of object)

It contains classes like File, Socket, Date, Time, etc... that let you manipulate files, send messages, tell the date and things of that nature. Mince, at the moment, is used only for console applications, but in theory could work for WinForms, game development, etc... Mince is only the language, libraries can be made for it to support those things, and making libraries is very easy.

## Getting Started
Mince is super simple, especially if you've programmed before. The classic "Hello World" example is as easy as two lines:

    console.print("Hello world!"); // prints hello world
    console.readKey(); // waits for keypress before ending program

## Variables
Creating variables is also rather simple.

    global myVar = "foo bar"; // this variable can now be accessed anywhere
    myOtherVar = 1 + 1; // variables without 'global' can only be accessed within their scope

    myVar = 25; // variables can be re-assigned like this
    free myVar; // now myVar no longer exists

## If statements
If statements are mostly the same as in other languages. Brackets between 'if' and '{' are optional.

    input = console.readLine();
    if input == "foo" {
        // do something here
    } else if input == "bar" {
        // do something else here
    } else {
        // do something even else here
    }

## Loops
Loops in Mince don't differ much from other languages either.
### While loops
While loops are the fastest kind of loops, and are recommended.

    i = 0;
    while i < 10 {
        // do something
        i++;
    }

### Until loops
Until loops are simply inverted while loops

    i = 0;
    until i == 10 {
        // do something here
        i++;
    }

If you printed the value of `i` each time, it should output every integer from 0 to 9

### Foreach loops
Foreach loops are the 2nd fastest kind of loop, and are done like this

    array = [ "a", "b", "c", "d" ]
    foreach item in array {
        // do something with item here
    }

### For loops
The slowest kind of loop, avoid these if possible

    for i = 0 : i < 10 : i++; {
        // do something here
    }

## Functions
Functions are a bit different from other languages, just follow the example below

    function add : x, y {
        return x + y;
    }
    
    add(10, 5); // should return 15

If you don't want any parameters, just omit that part from the function declaration.

    function myFunc {
        return "Hello world";
    }
    
    myFunc(); // should return Hello World

If you want to return nothing, just don't put a return statement inside the function, and null will be returned by default. You can also return null for flow control reasons.

    function parseNumber : myString {
        if !myString.isNumber() {
            return null;
        }
        return myString.toNumber();
    }
    
    parseNumber("this is a string"); // should return null
    parseNumber("100.56"); // should return 100.56

## Classes
Classes are very different from other languages in Mince

    class MyClass {
        
    }

At this point it looks normal, however once you want to add fields and things to the class it gets different.
### Fields

    class MyClass {
        public field myField = 40; // a public field
        private field secondField = [ 1, 20, 39, "abc" ]; // a private field
        field anotherField = "string"; // fields are private by default
    }

### Instantiating classes
Now to instantiate the class above that we just made, all we need to do is

    instantiated = new MyClass();

### Methods
Methods are also a bit different, but very similar to functions

    class MyClass {
        public field myField = 40; // a public field
        private field secondField = [ 1, 20, 39, "abc" ]; // a private field
        field anotherField = "string"; // fields are private by default

        // same syntax as functions, except replace 'function' with 'method'
        public method getSecondField { 
            return this.secondField;
        }
    }

Remember, to access a field inside a class you **must** type _this_ before the name.
### Constructors
Constructors are just methods named 'new'

    class MyClass {
        public field myField = 40; // a public field
        private field secondField = [ 1, 20, 39, "abc" ]; // a private field
        field anotherField = "string"; // fields are private by default

        // same syntax as functions, except replace 'function' with 'method'
        public method new : anotherField { 
            this.anotherField = anotherField;
        }
    }

And now it can be instantiated like this

    instantiated = new MyClass("a parameter");

### Properties
Mince also contains C#-like properties

    class MyClass {
        private field secretVariable = "shhh! secret!";
        public property notSoSecretProperty {
            getter {
                return this.secretVariable;
            }
            // If no setter is included, the variable is readonly
            // 'value' can be named to anything
            setter : value { 
                this.secretVariable = value;
            }
        }
    }

## Dynamics
Dynamics are a very powerful special kind of object. Variables can be created inside them on the fly.

    myDynamic = new Dynamic();
    myDynamic.myNewVariable = "foo bar"; // myNewVariable didn't exist until I created it
    myVar = myDynamic.thisDoesntExist; // However, trying to read a variable that doesn't exist will throw an error

Dynamics can be made in another way too, using some syntactic sugar. Doing it the following way will also let you declare variables inside the dynamic at instantiation.

    myDynamic = { 
        myNewVar = "foo bar"
    };

This can be used to return "multiple variables" from a function, even though you just return a dynamic

    function divide : dividend, divisor {
        return {
            quotient = dividend / divisor,
            remainder = dividend.mod(divisor)
        };
    }
    dummy = divide(10, 20).quotient; // dummy should equal 0.5
    dummy = divide(10, 20).remainder; // dummy should equal 10

## System Classes and Members
If you want to know what you can do with a string, or what console can do, just check out the source code. All classes are in the Types folder.

## Making Your Own Library
If you want to know how to make your own library, check out MinceLibrary in the source code. Drag the .dll you compile into the lib folder of any mince program or compiler and you're set!
