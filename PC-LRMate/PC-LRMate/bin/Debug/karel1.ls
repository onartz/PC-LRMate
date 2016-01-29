
   K A R E L  Translator          Version 34     
     File name: C:\ConfigFanuc\karel1.kl

   1 PROGRAM karel1 
   2 %NOBUSYLAMP 
   3  
   4 %NOLOCKGROUP 
   5  
   6 %NOPAUSE = ERROR + COMMAND + TPENABLE 
   7  
   8 %NOABORT = ERROR + COMMAND 
   9  
  10  
  11 VAR 
  12 REPLI, P1 : POSITION 
  13 BEGIN 
  14  $SPEED =1000 
  15  MOVE TO REPLI 
  16  $SPEED = 500 
  17  MOVE NEAR P1 BY 50 
  18  $MOTYPE = LINEAR 
  19  MOVE TO P1 
  20  MOVE AWAY 50 
  21  $MOTYPE = JOINT 
  22  $SPEED = 1000 
  23 MOVE TO REPLI 
  24 END karel1

*** Translation successful, 239 bytes of p-code generated, checksum 21728. ***
