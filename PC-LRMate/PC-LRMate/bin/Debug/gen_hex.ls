
   K A R E L  Translator          Version 34     
     File name: C:\ConfigFanuc\gen_hex.kl

   1  
   2 ------------------------------------------------------------------------------ 
   3 ---- Section 1: Program and Environment Declaration 
   4 ------------------------------------------------------------------------------ 
   5 PROGRAM gen_hex 
   6 %COMMENT = 'HEXAGON' 
   7  
   8 ------------------------------------------------------------------------------ 
   9 ---- Section 2: Constant and Variable Declaration 
  10 ------------------------------------------------------------------------------ 
  11 CONST 
  12  L_HEX_SIDE = 200-- Length of one side of the hexagon 
  13  NUM_AXES = 6 -- Number of robot axes 
  14 VAR 
  15  p_cntr : JOINTPOS6 -- Center of the hexagon 
  16  p_xyzwpr : ARRAY[NUM_AXES] OF XYZWPR 
  17  -- Six vertices of the hexagon 
  18  tmp_xyz :XYZWPR 
  19  clock, 
  20  t_start, 
  21  t_end, 
  22  t_total : INTEGER 
  23  status, 
  24  p_indx : INTEGER 
  25  
  26 ------------------------------------------------------------------------------ 
  27 ---- Section 3: Routine Declaration 
  28 ------------------------------------------------------------------------------ 
  29 ---------------------------------------------------------------------------- 
  30 ---- Section 3-A: Routines move_to_pr and movl_to_pr are TP 
  31 ---- routines for doing moves 
  32 ---------------------------------------------------------------------------- 
  33 ROUTINE move_to_pr FROM MOVE_TO_PR -- move_to_pr must also be loaded. 
  34  -- 1:J PR[1] 100% FINE ; 
  35 ROUTINE movl_to_pr FROM MOVL_TO_PR -- movl_to_pr must also be loaded. 
  36  -- 1:L PR[1] 1000mm/sec FINE ; 
  37 ------------------------------------------------------------------------------ 
  38 ---- Section 3-B: R_HEX_CENTER Declaration 
  39 ---- Calculates the hexagon points based on distance 
  40 ---- between point 1 and 4 of the hexagon. 
  41 ------------------------------------------------------------------------------ 
  42 ROUTINE r_calc_hex 
  43 VAR 
  44  p1_to_pcntr : REAL -- Distance from the center of the hex to point 1 
  45  vertice : INTEGER -- the index used specify each vertice of hexagon 
  46 BEGIN 
  47  p1_to_pcntr = (L_HEX_SIDE / 2) + (L_HEX_SIDE * COS(60)) 
  48  p_xyzwpr[1] = p_cntr -- p_cntr was calculated in r_hex_center 
  49  p_xyzwpr[1].y = p_xyzwpr[1].y - p1_to_pcntr --set the first vertice of hex 
  50  FOR vertice = 2 TO NUM_AXES DO -- start at 2 since 1 is already set 
  51    
  52  ENDFOR 
  53  -- Calculating individual components for each vertice of the hexagon 
  54  p_xyzwpr[2].x = p_xyzwpr[1].x + (L_HEX_SIDE * SIN(60)) 
  55  p_xyzwpr[2].y = p_xyzwpr[1].y + (L_HEX_SIDE * COS(60)) 
  56  p_xyzwpr[3].x = p_xyzwpr[1].x + (L_HEX_SIDE * SIN(60)) 
  57  p_xyzwpr[3].y = p_xyzwpr[1].y + (L_HEX_SIDE + (L_HEX_SIDE * COS(60))) 
  58  p_xyzwpr[4].y = p_xyzwpr[1].y + (L_HEX_SIDE + (2*L_HEX_SIDE * COS(60))) 
  59  p_xyzwpr[5].x = p_xyzwpr[1].x - (L_HEX_SIDE * SIN(60)) 
  60  p_xyzwpr[5].y = p_xyzwpr[3].y 
  61  p_xyzwpr[6].x = p_xyzwpr[1].x - (L_HEX_SIDE * SIN(60)) 
  62  p_xyzwpr[6].y = p_xyzwpr[2].y 
  63 END r_calc_hex 
  64  
  65 ----------------------------------------------------------------------------- 
  66 ---- Section 3-C: R_HEX_CENTER Declaration 
  67 ---- Positions the face plate perpendicular 
  68 ---- to the xy world coordinate plane. 
  69 ----------------------------------------------------------------------------- 
  70 ROUTINE r_hex_center 
  71 VAR 
  72  status, indx : INTEGER 
  73  p_cntr_arry : ARRAY[NUM_AXES] OF REAL 
  74 BEGIN 
  75  -- Initalize the center position array to zero 
  76  FOR indx = 1 TO NUM_AXES DO 
  77   p_cntr_arry[indx] = 0 
  78  ENDFOR 
  79  -- Set JOINT 3 and 5 to -45 and 45 degrees 
  80  p_cntr_arry[3] = -45 
  81  p_cntr_arry[5] = 45  
  82  -- Convert the REAL array to a joint position, 
  83  -- p_cntr 
  84  CNV_REL_JPOS(p_cntr_arry, p_cntr, status) 
  85  SET_JPOS_REG(1, p_cntr, status) -- Put p_cntr in PR[1] 
  86  move_to_pr -- Call TP program to move to PR[1] 
  87 END r_hex_center 
  88  
  89 ----------------------------------------------------------------------------- 
  90 ---- Section 4: Main Program 
  91 ----------------------------------------------------------------------------- 
  92 BEGIN  
  93  ----------------------------------------------------------------------------- 
  94  ---- Section 4-A: Connect timer, set uframe, call routines 
  95  ----------------------------------------------------------------------------- 
  96  clock = 0 -- Initialize clock value to zero 
  97  CONNECT TIMER TO clock -- Connect the timer 
  98  WRITE ('Moving to the center of the HEXAGON',CR) -- update user of process 
  99  r_hex_center -- position the face plate of robot. 
 100  WRITE ('Calculating the sides of HEXAGON',CR) -- update user 
 101  r_calc_hex -- Calculate the hexagon points 
 102  ----------------------------------------------------------------------------- 
 103  ---- Section 4-B: Move on sides of hexagon 
 104  ----------------------------------------------------------------------------- 
 105  WRITE ('Moving along the sides of the Hexagon',CR) -- Update user 
 106  t_start = clock -- Record the time before motion begins 
 107  FOR p_indx = 1 TO 6 DO 
 108   -- Verify that the position is reachable 
 109   CHECK_EPOS ((p_xyzwpr[p_indx]), $UFRAME, $UTOOL, status) 
 110   IF (status <> 0) THEN 
 111    WRITE ('Unable to move to p_xyzwpr[', p_indx,']',CR); 
 112   ELSE 
 113    SET_POS_REG(1, p_xyzwpr[p_indx], status) 
 114    move_to_pr  -- Call TP program to move to PR[1] 
 115   -- Move to each vertex of hexagon 
 116   ENDIF 
 117  ENDFOR 
 118  SET_POS_REG(1, p_xyzwpr[1], status) 
 119  move_to_pr  -- Call TP program to move to PR[1] 
 120  -- Move back to first vertex of hexagon 
 121  tmp_xyz = p_cntr 
 122  SET_POS_REG(1, tmp_xyz, status) 
 123  move_to_pr  -- Move TCP in a straight 
 124  -- line to the center position 
 125  t_end = clock -- Record ending time 
 126  WRITE('Total motion time = ',t_end-t_start,CR) --Display the total time for 
 127  -- motion. 
 128   
 129 END GEN_HEX 
 130  

*** Translation successful, 1555 bytes of p-code generated, checksum 57336. ***
