# Elevator
A simulator of an elevator to be automated by a PLC (or PLC simulator)

**DummyPlcPlugin** is an example of automation plugin

![alt text](https://raw.githubusercontent.com/cobrce/Elevator/master/Elevator/2018-06-24_20-00-30.gif)

## How to make your own plugin
* Make an implementation of IPLC that handles the logic or the communication with a plc/simulator
* Make an implementation of Plugins.IPlugin (or Plugins.AbstractPlugin)
* Copy the compiled assembly to "Plugins" subdirectory

## Todo
* Write a plugin for step7 plcsim
