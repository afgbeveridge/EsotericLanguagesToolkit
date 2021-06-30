USE esoteric_languages;
LOAD DATA INFILE '/Languages.csv'  INTO TABLE Languages 
FIELDS TERMINATED 
BY ',' 
LINES TERMINATED 
BY '\n' 
IGNORE 1 ROWS
(Name, Hash, IsNativelySupported)