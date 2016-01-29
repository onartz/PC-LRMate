
   K A R E L  Translator          Version 34     
     File name: C:\ConfigFanuc\exo1.kl

   1 PROGRAM EXO1 
   2  
   3 %ALPHABETIZE             -- Create the variables in alphabetical order 
   4 %COMMENT     = 'PATH MOVES' 
   5 %ENVIRONMENT PATHOP      -- Necessary for PATH_LEN 
   6 %ENVIRONMENT SYSDEF      -- Necessary for using the $MOTYPE in the MOVEs 
   7 %ENVIRONMENT UIF         -- Necessary for SET_CURSOR 
   8 ----------------------------------------------------------------------------- 
   9 ----    Section 2:  Constant and Variable Declarations 
  10 ----------------------------------------------------------------------------- 
  11 CONST 
  12     CH_ABORT   = 1                           -- Number associated with the 
  13                                              -- abort Condition handler 
  14     CH_F1      = 2                           -- Number associated with the 
  15                                              -- F1 key Condition handler 
  16 VAR 
  17     STATUS :INTEGER             -- Status from built-in calls 
  18     node_ind            :INTEGER             -- Index used when moving along path 
  19     loop_pth            :INTEGER             -- Used in a FOR loop counter 
  20     prg_abrt            :BOOLEAN             -- Set when program is aborted 
  21     pth1                :PATH               
  22     strt_jnt            :JOINTPOS6           -- starting position of a move 
  23     via_pos             :XYZWPR              -- via point for a circular move 
  24     des_pos             :XYZWPR              -- destination point 
  25     real_ary            :ARRAY[6] OF REAL    -- This is used for creating 
  26                                              -- a joint position with 6 axes 
  27    indx              :INTEGER             -- FOR loop counter 
  28   prog_index : INTEGER 
  29      
  30     ----------------------------------------------------------------------------- 
  31 ----    Section 3:  Routine Declaration 
  32 ----------------------------------------------------------------------------- 
  33 ----    Section 3-A:  TP_CLS Declaration 
  34 ----     ROUTINE TP_CLS FROM ROUT_EX        -- ROUT_EX must also be loaded. 
  35 ----------------------------------------------------------------------------- 
  36 ----------------------------------------------------------------------------- 
  37 ----    Section 3-B:  YES_NO Declaration 
  38 ---                  Display choices on the function line of the TP. 
  39 ----                  Asks for user response. 
  40 ---                  F1 key is monitored by the Global condition handler 
  41 ----                 [CH_F1] and the F2 is monitored here. 
  42 ----                 If F1 is pressed the program will abort. 
  43 ---                  But, if the F2 is pressed the program will continue. 
  44 ----------------------------------------------------------------------------- 
  45 ROUTINE YES_NO 
  46 BEGIN 
  47   WRITE TPFUNC (CHR(137))                  -- Home Cursor in Function window 
  48   WRITE TPFUNC ('  ABORT  CONT')           -- Display Function key options 
  49   WAIT FOR TPIN[131]                       -- Wait for user to respond to 
  50                                            -- continue.  If the user presses 
  51                                            -- F1 (abort) condition handler 
  52                                            -- CH_ABORT will abort program. 
  53   WRITE TPFUNC (CHR(137))                  -- Home Cursor in Function window 
  54   WRITE TPFUNC ('  ABORT',CHR(129))        -- Redisplay just Abort option and 
  55                                            -- clear rest of Function window 
  56 END YES_NO 
  57  
  58 ROUTINE TP_CLS FROM rout_ex 
  59  
  60 BEGIN 
  61    CONDITION[CH_ABORT]: 
  62    WHEN ABORT DO       -- When the program is aborting set prg_abrt flag. 
  63                       -- This will be triggered if this program aborts itself 
  64                       --  or if an external mechanism aborts this program. 
  65       prg_abrt = TRUE   -- You may then have another task which detects 
  66                        -- prg_abrt being set, and does shutdown operations 
  67                       -- (ie: set DOUT/GOUT's, send signals to a PLC) 
  68    ENDCONDITION 
  69     
  70    CONDITION[CH_F1]: 
  71       WHEN TPIN[129] DO   -- Monitor TP 'F1' Key. If 'F1' key is pressed, 
  72        ABORT             -- abort the program. 
  73    ENDCONDITION 
  74 prg_abrt = FALSE                -- Initialize variable which is set only if 
  75                                 -- the program is aborted and CH_ABORT is 
  76                                 -- enabled. 
  77 ENABLE CONDITION[CH_ABORT]      -- Start scanning abort condition as defined. 
  78 ENABLE CONDITION[CH_F1]         -- Start scanning F1 key condition as defined. 
  79   
  80 ----------------------------------------------------------------------------- 
  81 ----    Section 4-B: Display banner message and wait for users response 
  82 ----------------------------------------------------------------------------- 
  83 TP_CLS                                -- Routine Call; Clears the TP USER 
  84                                       -- menu, and forces the TP USER menu 
  85                                       -- to be visible. 
  86 SET_CURSOR(TPDISPLAY,2,13, STATUS)   -- Set cursor position in TP USER menu 
  87 IF (STATUS <> 0 ) THEN               -- Verify that SET_CURSOR was successful 
  88    WRITE ('SET_CURSOR built-in failed with status = ',STATUS,CR) 
  89    YES_NO                            -- Ask whether to quit, due to error. 
  90 ENDIF 
  91 --- Write heading in REVERSE video, then turn reverse video off 
  92 WRITE (CHR(139),' PLEASE READ ',CHR(143),CR) 
  93 WRITE (CR,' *** F1 Key is labelled as ABORT key *** ') 
  94 WRITE (CR,' Any time the F1 key is pressed the program') 
  95 WRITE (CR,' will abort. However, the F2 key is active ') 
  96 WRITE (CR,' only when the function key is labeled.',CR,CR) 
  97 YES_NO  -- Wait for user response 
  98  
  99 ---------------------------------------------------------------------------- 
 100 ----    Section 4-D: Creating a joint position and moving along paths 
 101 ---------------------------------------------------------------------------- 
 102 FOR indx = 1 TO 6 DO                      -- Set all joint angles to zero 
 103     real_ary[indx] = 0.0 
 104 ENDFOR 
 105 real_ary[5] = 90.0                        -- Make sure that the position 
 106                                           -- is not at a singularity point. 
 107 CNV_REL_JPOS(real_ary, strt_jnt, STATUS)  -- Convert real_ary values into 
 108                                           -- a joint position, strt_jnt 
 109 IF (STATUS <> 0 ) THEN                    -- Converting joint position 
 110                                           -- was NOT successful 
 111    WRITE ('CNV_REL_JPOS built-in failed with status = ',STATUS,CR) 
 112    YES_NO                                 -- Ask user if want to continue. 
 113 ELSE                                      -- Converting joint position was 
 114                                           -- successful.  
 115    
 116    
 117  
 118     CALL_PROG('prog4', prog_index)        
 119  
 120   ENDIF 
 121  
 122   
 123  
 124   WRITE ('pth_move Successfully Completed',CR) 
 125  
 126  
 127     
 128 END EXO1 
 129  

*** Translation successful, 1072 bytes of p-code generated, checksum 24743. ***
