'''__Name__''' is an object and stack based language, created by [[User:__User__|User:__User__]] ([[User talk:__User__|talk]]) in May 2013. All numerics are signed, integral and expressed in hexatridecimal (base 36) notation, unless the radix system is changed within an executing program. WARP is a (weak) recursive acronym, WARP And Run Program - so called because it is expected that interpreters randomize 
the source program after each command is executed.
 
==Objects and expressions==
Basic pseudo grammatical terms:
* <object> ::= [a-z]{2}
* <sobject> ::= <object> | !
* <base-expr> ::= -{0,1}[0-9,A-Z]+ | <object> | "...." | ~ | `
* <expr> ::= <base-expr> | ! | _
* <label> ::= [a-z]+ | .

==Operators==
A few general purpose operators exist.
{| class="wikitable"
!Command
!Description
|-
|%%Assignment%%&lt;object&gt;&lt;expr&gt;Radix
|assign &lt;expr&gt; to &lt;object&gt;
|-
|%%Addition%%&lt;sobject&gt;&lt;expr&gt;
|increment &lt;sobject&gt; by &lt;expr&gt; and update &lt;sobject&gt;. If &lt;expr&gt; is non numeric, convert if possible, otherwise treat as 

0. If &lt;sobject&gt; is the pop command, use the stack as the source object and push the result onto the stack   
|-
|%%Subtraction%%&lt;sobject&gt;&lt;expr&gt;
|Decrement &lt;sobject&gt; by &lt;expr&gt; and update &lt;sobject&gt;. If &lt;expr&gt; is non numeric, convert if possible, otherwise treat as 

0
|-
|%%Division%%&lt;sobject&gt;&lt;expr&gt;
|Divide &lt;sobject&gt; by &lt;expr&gt; and update &lt;sobject&gt;. If &lt;expr&gt; is non numeric, convert if possible, or if 0, treat it as 

1
|-
|%%Multiplication%%amp;&lt;sobject&gt;&lt;expr&gt;
|Multiply &lt;sobject&gt; by &lt;expr&gt; and update &lt;sobject&gt;. If &lt;expr&gt; is non numeric, convert if possible, otherwise treat as 

1
|-
|%%Modulo%% &lt;sobject&gt;&lt;expr&gt;
|Calculate &lt;sobject&gt; mod &lt;expr&gt; and update &lt;sobject&gt;. 
|-
|%%Comparison%%&lt;expr&gt;:&lt;expr&gt;
|Compare the first &lt;expr&gt; with the second &lt;expr&gt;. Push the result of the comparison onto the stack. 
0 means equal, -1 is less than, 1 greater than 
|-
|%%ConditionalExecution%%&lt;expr&gt;?&lt;command&gt;
|Pop from the stack, and if that object equals &lt;expr&gt;, execute &lt;command&gt;.
|-
|<code><nowiki>%%RadixSwitch%%</nowiki></code>&lt;number&gt;
|Change the radix of numbers to &lt;number&gt; from now on
|}

==Stack manipulation==
There are several commands for stack manipulation.
{| class="wikitable"
!Command
!Description
|-
|%%Treat%%&lt;object&gt;
|treat &lt;object&gt; as the stack; takes string of content and push each atomic object onto the stack - pushed so that repeated popping and 

sending to output is the same object as 'treated'
|-
|<nowiki>%%Untreat%%</nowiki>
|untreat the current stack
|-
|%%Pop%%
|pop
|-
|%%Push%%&lt;base-expr&gt;
|push &lt;base-expr&gt;
|-
|%%PopAndPush%%&lt;base-expr&gt;
|Pop and push &lt;base-expr&gt;
|-
|\
|Turn off auto stacking mode
|-
|%%DuplicateTOS%%
|Duplicate the top of stack
|-
|%%RotateStacks%%
|Rotate the top two stacks in the stack of stacks
|-
|<code><nowiki>%%RetrieveFromRAS%%</nowiki></code>&lt;base-expr&gt;
|Retrieve the object at index &lt;base-expr&gt; from the random access stack and push onto the current stack
|-
|<code><nowiki>%%PlaceInRAS%%</nowiki></code>&lt;base-expr&gt;
|Place an object popped from the current stack at index &lt;base-expr&gt; in the random access stack
|}

==Labels and movement==
WARP supports a simple label/jump set of commands.
{| class="wikitable"
!Command
!Description
|-
|%%Label%%&lt;label&gt;
|Declare a named label
|-
|%%Jump%%&lt;expr&gt;&lt;&lt;label&gt;&lt;
|If &lt;expr&gt; == ., move to label unconditionally. If &lt;expr&gt; is zero or false, or represents an empty object, fall through
|}

==Input and output==
__Name__ can interact with any existing input and output mechanisms.
{| class="wikitable"
!Command
!Description
|-
|%%OutputCharacter%%&lt;expr&gt;
|Output &lt;expr&gt; as a character
|-
|%%OutputNativeForm%%&lt;expr&gt;
|Output &lt;expr&gt; in its native form
|-
|%%Input%%l
|Accept a line of user input and place onto the stack
|-
|%%Input%%c
|Accept a character of user input and place onto the stack
|-
|%%CurrentStack%%
|a symbolic reference to the current stack
|}

==Source access==
Access to the source is provided as well as the current traversal map.
{| class="wikitable"
!Command
!Description
|-
|%%Quine%%
|The current randomized source
|-
|`
|The current traversal map of the randomized source
|}

==Environment and execution==
First, the run time environment.
===Environment===
__Name__ has initially one, unbound or system, stack. Using the % operator, a bound object can usurp the unbound stack, until the | operator 
removes it. Think of it is as being similar to a stack of stacks. Issuing a | operator against the unbound stack will mean that the 
environment enters 'stackless' mode. All further stack based operations will not operate as expected, nor generate an error. There is no 
formal limit to the size and number of stacks and similarly in relation to source code.

In addition, the __Name__ environment has a single random access stack, manipulated with the { and } operators.

Note that all __Name__ objects are shared across the stack of stacks; if an assignment to an object xx occurs, and then a new stack 
is created, the value of xx will be available in the new stack context.

Compliant interpreters will auto create objects on demand, initialized to zero. 

===Execution===
Each source advance generates a random rewrite of the source, along with an attendant 'unscramble' map. Could be used for amusing output 
effects.

==Examples==

Hello World
 %%OutputNativeForm%%"Hello World!"

Quine
 %%OutputNativeForm%%~

Primes less than some user entered value
 %%RadixSwitch%%A%%OutputNativeForm%%"Enter start: "%%Input%%l=cu!)"Primes <= ")cu)"\n":cu:2?-1?^.e@o*cu$!2=ca!@i*cu#!ca?0?^.n<ca1:ca:1?1?^.i)cu)" "@n<cu1*cu<!1^!o@e

Reverse a user entered string
 )"Enter a string to reverse: ",l=st!%st@r=ch!'*ch'^_r'@p)!^_p|


==External resources==
*[https://github.com/afgbeveridge/EsotericLanguagesToolkit An esoteric language toolkit with some standard interpreters, along with source code]

[[Category:Languages]]
[[Category:Object-oriented paradigm]]
[[Category:2013]]
[[Category:Implemented]]
