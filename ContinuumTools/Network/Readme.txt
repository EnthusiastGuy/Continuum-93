Client <-> Server protocol
==========================

Client can issue the following commands:
- get registers: server responds with all register values
- get memory;

Client can receive the following commands:
- view registers;
- view memory;






Examples:
==============



1. Getting registers

Client:
{
	oper: "getRegs",
	time: "1681217855"
}

Server:
{
	oper: "getRegs",
	time: "1681217855",
	data: [0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0]
}




2. Getting a part of the memory

Client:
{
	oper: "getMem: 0, 100",
	time: "1681217855"
}

Server:
{
	oper: "getMem",
	time: "1681217855",
	data: [0, 0, 0, ..., 0, 0, 0]
}
