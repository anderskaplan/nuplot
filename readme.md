NuPlot
======
NuPlot is a plotting library for WPF aiming to be for WPF what NPlot was for 
Windows Forms. That is, a no-frills, fast and simple plotting library focusing 
on the data and not on the prettiness -- the "chartjunk" in Edward Tufte's 
words.

That said, the project in its current state certainly has some way left to reach 
the goal. It's not mature yet, but it can be useful nevertheless.


License (MIT)
-------------
Copyright (C) 2014, Anders Kaplan.

Permission is hereby granted, free of charge, to any person obtaining a copy of 
this software and associated documentation files (the "Software"), to deal in 
the Software without restriction, including without limitation the rights to 
use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
the Software, and to permit persons to whom the Software is furnished to do so, 
subject to the following conditions:

The above copyright notice and this permission notice shall be included in all 
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS 
FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR 
COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER 
IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN 
CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.


Backlog
-------
Make sure that axes with decreasing/non-increasing ranges are handled properly.
Automated builds.
Auto-assign line/marker colors for plots with multiple series (optionally).
Handle cultures properly.

Support Silverlight in addition to WPF.		
Data binding: additional tests.
XmlDocs/IntelliSense.
Point plot: support a variety of marker shapes.
Expose fonts etc as dependency properties.
Add a line plot to the sinc demo, like NPlot.
Design description.
Developers' manual.
LinearAxis.ChooseLargeTickStep - ugly code with TODO needs testing & refactoring
Step plot: "Center" flag, like NPlot.
Improve DataContext forwarding on the PlotCollection?
LinearAxis: converter object (as parameter) which can convert from e.g. TimeSpan to double		
