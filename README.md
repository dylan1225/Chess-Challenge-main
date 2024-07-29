API and framework from Sebastian Lague

My engine is in Mybot.cs located at chess-challenge-main/chess-challenge/src/mybot/mybot.cs

All I used was the API. The framework was not UCI compatible and ran in a GUI, so I had to make it UCI compatible; the API is similar to the Python chess library.
I made it UCI compatible, found in src/framework/application/challengecontroller.cs and program.cs.

You can find an exe file I already compiled in chess-challenge-main

Ignore the main.py. I tried to code this in Python, but I find myself more familiar with c#

When loaded into Arena, please check to configure my engine to type UCI instead of winboard, for whatever reason, it automatically sets it to winboard when I install it in arena./

