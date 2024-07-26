*** Settings ***
Library    SerialLibrary
Library    OperatingSystem


*** Test Cases ***
Scenario: Power Failure Test --> default settings 
    Given I connect to COM4 with baudrate 9600
    When I start to write with the default settings
    And I restart the computer
    And I wait for the computer to restart
    Then I can analyze the corrupted files with the default settings
# --------------------------------------------------------------------------------------------------------------------- #
Scenario: Power Failure Test --> size: 1ko & WriteThrough method
    Given I connect to COM4 with baudrate 9600
    When I start to write with a size of 1ko and with the WriteThrough method and with the iteration fifth and with 200 maxtx
    And I restart the computer
    And I wait for the computer to restart
    Then I can analyze the corrupted files with a size of 1ko and with the WriteThrough method and with the iteration fifth

Scenario: Power Failure Test --> size: 100ko & WriteThrough method
    Given I connect to COM4 with baudrate 9600
    When I start to write with a size of 100ko and with the WriteThrough method and with the iteration tenth and with 200 maxtx
    And I restart the computer
    And I wait for the computer to restart
    Then I can analyze the corrupted files with a size of 100ko and with the WriteThrough method and with the iteration tenth

Scenario: Power Failure Test --> size: 10Mo & WriteThrough method
    Given I connect to COM4 with baudrate 9600
    When I start to write with a size of 10Mo and with the WriteThrough method and with the iteration tenth and with 100 maxtx
    And I restart the computer
    And I wait for the computer to restart
    Then I can analyze the corrupted files with a size of 10Mo and with the WriteThrough method and with the iteration tenth

Scenario: Power Failure Test --> size: 50Mo & WriteThrough method
    Given I connect to COM4 with baudrate 9600
    When I start to write with a size of 50Mo and with the WriteThrough method and with the iteration tuesday and with 50 maxtx
    And I restart the computer
    And I wait for the computer to restart
    Then I can analyze the corrupted files with a size of 50Mo and with the WriteThrough method and with the iteration tuesday

Scenario: Power Failure Test --> size: 100Mo & WriteThrough method
    Given I connect to COM4 with baudrate 9600
    When I start to write with a size of 100Mo and with the WriteThrough method and with the iteration monday and with 25 maxtx
    And I restart the computer
    And I wait for the computer to restart
    Then I can analyze the corrupted files with a size of 100Mo and with the WriteThrough method and with the iteration monday
# --------------------------------------------------------------------------------------------------------------------- #
Scenario: Power Failure Test --> size: 1ko & None method
    Given I connect to COM4 with baudrate 9600
    When I start to write with a size of 1ko and with the None method and with the iteration tenth and with 200 maxtx
    And I restart the computer
    And I wait for the computer to restart
    Then I can analyze the corrupted files with a size of 1ko and with the None method and with the iteration tenth

Scenario: Power Failure Test --> size: 100ko & None method
    Given I connect to COM4 with baudrate 9600
    When I start to write with a size of 100ko and with the None method and with the iteration tenth and with 200 maxtx
    And I restart the computer
    And I wait for the computer to restart
    Then I can analyze the corrupted files with a size of 100ko and with the None method and with the iteration tenth

Scenario: Power Failure Test --> size: 10Mo & None method
    Given I connect to COM4 with baudrate 9600
    When I start to write with a size of 10Mo and with the None method and with the iteration tenth and with 100 maxtx
    And I restart the computer
    And I wait for the computer to restart
    Then I can analyze the corrupted files with a size of 10Mo and with the None method and with the iteration tenth
    
Scenario: Power Failure Test --> size: 50Mo & None method
    Given I connect to COM4 with baudrate 9600
    When I start to write with a size of 50Mo and with the None method and with the iteration tenth and with 50 maxtx
    And I restart the computer
    And I wait for the computer to restart
    Then I can analyze the corrupted files with a size of 50Mo and with the None method and with the iteration tenth

Scenario: Power Failure Test --> size: 100Mo & None method
    Given I connect to COM4 with baudrate 9600
    When I start to write with a size of 100Mo and with the None method and with the iteration tenth and with 25 maxtx
    And I restart the computer
    And I wait for the computer to restart
    Then I can analyze the corrupted files with a size of 100Mo and with the None method and with the iteration tenth
# --------------------------------------------------------------------------------------------------------------------- #
Scenario: Power Failure Test --> size: 1ko & Asynchronous method
    Given I connect to COM4 with baudrate 9600
    When I start to write with a size of 1ko and with the Asynchronous method and with the iteration tenth and with 200 maxtx
    And I restart the computer
    And I wait for the computer to restart
    Then I can analyze the corrupted files with a size of 1ko and with the Asynchronous method and with the iteration tenth

Scenario: Power Failure Test --> size: 100ko & Asynchronous method
    Given I connect to COM4 with baudrate 9600
    When I start to write with a size of 100ko and with the Asynchronous method and with the iteration tenth and with 200 maxtx
    And I restart the computer
    And I wait for the computer to restart
    Then I can analyze the corrupted files with a size of 100ko and with the Asynchronous method and with the iteration tenth

Scenario: Power Failure Test --> size: 10Mo & Asynchronous method
    Given I connect to COM4 with baudrate 9600
    When I start to write with a size of 10Mo and with the Asynchronous method and with the iteration tenth and with 100 maxtx
    And I restart the computer
    And I wait for the computer to restart
    Then I can analyze the corrupted files with a size of 10Mo and with the Asynchronous method and with the iteration tenth

Scenario: Power Failure Test --> size: 50Mo & Asynchronous method
    Given I connect to COM4 with baudrate 9600
    When I start to write with a size of 50Mo and with the Asynchronous method and with the iteration tenth and with 50 maxtx
    And I restart the computer
    And I wait for the computer to restart
    Then I can analyze the corrupted files with a size of 50Mo and with the Asynchronous method and with the iteration tenth

Scenario: Power Failure Test --> size: 100Mo & Asynchronous method
    Given I connect to COM4 with baudrate 9600
    When I start to write with a size of 100Mo and with the Asynchronous method and with the iteration tenth and with 25 maxtx
    And I restart the computer
    And I wait for the computer to restart
    Then I can analyze the corrupted files with a size of 100Mo and with the Asynchronous method and with the iteration tenth

Scenario: restart the computer
    Given I connect to COM4 with baudrate 9600
    When I restart the computer
# --------------------------------------------------------------------------------------------------------------------- #
*** Keywords ***
I connect to ${port} with baudrate ${baudrate}
    Connect    ${port}    ${baudrate}
    Sleep    2

I start to write with the default settings
    Run    python C:\\Users\\guillotn\\_work\\git\\poweroffrobot\\src\\Test_Client\\client.py writetx 
# --------------------------------------------------------------------------------------------------------------------- #    
I start to write with a size of 1ko and with the WriteThrough method and with the iteration ${iteration} and with ${maxtx} maxtx
    Run    python C:\\Users\\guillotn\\_work\\git\\poweroffrobot\\src\\Test_Client\\client.py writetx --size 1000 --method WriteThrough --iteration ${iteration} --maxtx ${maxtx}
    Sleep    30s
I start to write with a size of 100ko and with the WriteThrough method and with the iteration ${iteration} and with ${maxtx} maxtx
    Run    python C:\\Users\\guillotn\\_work\\git\\poweroffrobot\\src\\Test_Client\\client.py writetx --size 100000 --method WriteThrough --iteration ${iteration} --maxtx ${maxtx}
    Sleep    60s
I start to write with a size of 10Mo and with the WriteThrough method and with the iteration ${iteration} and with ${maxtx} maxtx
    Run    python C:\\Users\\guillotn\\_work\\git\\poweroffrobot\\src\\Test_Client\\client.py writetx --size 10000000 --method WriteThrough --iteration ${iteration} --maxtx ${maxtx}
    Sleep   70s
I start to write with a size of 50Mo and with the WriteThrough method and with the iteration ${iteration} and with ${maxtx} maxtx
    Run    python C:\\Users\\guillotn\\_work\\git\\poweroffrobot\\src\\Test_Client\\client.py writetx --size 50000000 --method WriteThrough --iteration ${iteration} --maxtx ${maxtx}
    Sleep    75s
I start to write with a size of 100Mo and with the WriteThrough method and with the iteration ${iteration} and with ${maxtx} maxtx
    Run    python C:\\Users\\guillotn\\_work\\git\\poweroffrobot\\src\\Test_Client\\client.py writetx --size 100000000 --method WriteThrough --iteration ${iteration} --maxtx ${maxtx}
    Sleep    130s
# --------------------------------------------------------------------------------------------------------------------- #

I start to write with a size of 1ko and with the None method and with the iteration ${iteration} and with ${maxtx} maxtx
    Run    python C:\\Users\\guillotn\\_work\\git\\poweroffrobot\\src\\Test_Client\\client.py writetx --size 1000 --method None --iteration ${iteration} --maxtx ${maxtx}
    Sleep    30s
I start to write with a size of 100ko and with the None method and with the iteration ${iteration} and with ${maxtx} maxtx
    Run    python C:\\Users\\guillotn\\_work\\git\\poweroffrobot\\src\\Test_Client\\client.py writetx --size 100000 --method None --iteration ${iteration} --maxtx ${maxtx}
    Sleep    60s
I start to write with a size of 10Mo and with the None method and with the iteration ${iteration} and with ${maxtx} maxtx
    Run    python C:\\Users\\guillotn\\_work\\git\\poweroffrobot\\src\\Test_Client\\client.py writetx --size 10000000 --method None --iteration ${iteration} --maxtx ${maxtx}
    Sleep    200s
I start to write with a size of 50Mo and with the None method and with the iteration ${iteration} and with ${maxtx} maxtx
    Run    python C:\\Users\\guillotn\\_work\\git\\poweroffrobot\\src\\Test_Client\\client.py writetx --size 50000000 --method None --iteration ${iteration} --maxtx ${maxtx}
    Sleep    130s
I start to write with a size of 100Mo and with the None method and with the iteration ${iteration} and with ${maxtx} maxtx
    Run    python C:\\Users\\guillotn\\_work\\git\\poweroffrobot\\src\\Test_Client\\client.py writetx --size 100000000 --method None --iteration ${iteration} --maxtx ${maxtx}
    Sleep    520s
# --------------------------------------------------------------------------------------------------------------------- #
I start to write with a size of 1ko and with the Asynchronous method and with the iteration ${iteration} and with ${maxtx} maxtx
    Run    python C:\\Users\\guillotn\\_work\\git\\poweroffrobot\\src\\Test_Client\\client.py writetx --size 1000 --method Asynchronous --iteration ${iteration} --maxtx ${maxtx}
    Sleep    30s
I start to write with a size of 100ko and with the Asynchronous method and with the iteration ${iteration} and with ${maxtx} maxtx
    Run    python C:\\Users\\guillotn\\_work\\git\\poweroffrobot\\src\\Test_Client\\client.py writetx --size 100000 --method Asynchronous --iteration ${iteration} --maxtx ${maxtx}
    Sleep    60s
I start to write with a size of 10Mo and with the Asynchronous method and with the iteration ${iteration} and with ${maxtx} maxtx
    Run    python C:\\Users\\guillotn\\_work\\git\\poweroffrobot\\src\\Test_Client\\client.py writetx --size 10000000 --method Asynchronous --iteration ${iteration} --maxtx ${maxtx}
    Sleep    200s
I start to write with a size of 50Mo and with the Asynchronous method and with the iteration ${iteration} and with ${maxtx} maxtx
    Run    python C:\\Users\\guillotn\\_work\\git\\poweroffrobot\\src\\Test_Client\\client.py writetx --size 50000000 --method Asynchronous --iteration ${iteration} --maxtx ${maxtx}
    Sleep    130s
I start to write with a size of 100Mo and with the Asynchronous method and with the iteration ${iteration} and with ${maxtx} maxtx
    Run    python C:\\Users\\guillotn\\_work\\git\\poweroffrobot\\src\\Test_Client\\client.py writetx --size 100000000 --method Asynchronous --iteration ${iteration} --maxtx ${maxtx}
    Sleep    130s
# --------------------------------------------------------------------------------------------------------------------- #

I restart the computer
    Write    RESTART

I wait for the computer to restart
    Sleep    50s

I can analyze the corrupted files with the default settings
    Run    python C:\\Users\\guillotn\\_work\\git\\poweroffrobot\\src\\Test_Client\\client.py analyze
# --------------------------------------------------------------------------------------------------------------------- #
I can analyze the corrupted files with a size of 1ko and with the WriteThrough method and with the iteration ${iteration}
    Run    python C:\\Users\\guillotn\\_work\\git\\poweroffrobot\\src\\Test_Client\\client.py analyze --size 1000 --method WriteThrough --iteration ${iteration}
I can analyze the corrupted files with a size of 100ko and with the WriteThrough method and with the iteration ${iteration}
    Run    python C:\\Users\\guillotn\\_work\\git\\poweroffrobot\\src\\Test_Client\\client.py analyze --size 100000 --method WriteThrough --iteration ${iteration}
I can analyze the corrupted files with a size of 10Mo and with the WriteThrough method and with the iteration ${iteration}
    Run    python C:\\Users\\guillotn\\_work\\git\\poweroffrobot\\src\\Test_Client\\client.py analyze --size 10000000 --method WriteThrough --iteration ${iteration}
I can analyze the corrupted files with a size of 50Mo and with the WriteThrough method and with the iteration ${iteration}
    Run    python C:\\Users\\guillotn\\_work\\git\\poweroffrobot\\src\\Test_Client\\client.py analyze --size 50000000 --method WriteThrough --iteration ${iteration}
I can analyze the corrupted files with a size of 100Mo and with the WriteThrough method and with the iteration ${iteration}
    Run    python C:\\Users\\guillotn\\_work\\git\\poweroffrobot\\src\\Test_Client\\client.py analyze --size 100000000 --method WriteThrough --iteration ${iteration}
# --------------------------------------------------------------------------------------------------------------------- #
I can analyze the corrupted files with a size of 1ko and with the None method and with the iteration ${iteration}
    Run    python C:\\Users\\guillotn\\_work\\git\\poweroffrobot\\src\\Test_Client\\client.py analyze --size 1000 --method None --iteration ${iteration}
I can analyze the corrupted files with a size of 100ko and with the None method and with the iteration ${iteration}
    Run    python C:\\Users\\guillotn\\_work\\git\\poweroffrobot\\src\\Test_Client\\client.py analyze --size 100000 --method None --iteration ${iteration}
I can analyze the corrupted files with a size of 10Mo and with the None method and with the iteration ${iteration}
    Run    python C:\\Users\\guillotn\\_work\\git\\poweroffrobot\\src\\Test_Client\\client.py analyze --size 10000000 --method None --iteration ${iteration}
I can analyze the corrupted files with a size of 50Mo and with the None method and with the iteration ${iteration}
    Run    python C:\\Users\\guillotn\\_work\\git\\poweroffrobot\\src\\Test_Client\\client.py analyze --size 50000000 --method None --iteration ${iteration}
I can analyze the corrupted files with a size of 100Mo and with the None method and with the iteration ${iteration}
    Run    python C:\\Users\\guillotn\\_work\\git\\poweroffrobot\\src\\Test_Client\\client.py analyze --size 100000000 --method None --iteration ${iteration}
# --------------------------------------------------------------------------------------------------------------------- #
I can analyze the corrupted files with a size of 1ko and with the Asynchronous method and with the iteration ${iteration}
    Run    python C:\\Users\\guillotn\\_work\\git\\poweroffrobot\\src\\Test_Client\\client.py --size 1000 --method Asynchronous --iteration ${iteration}
I can analyze the corrupted files with a size of 100ko and with the Asynchronous method and with the iteration ${iteration}
    Run    python C:\\Users\\guillotn\\_work\\git\\poweroffrobot\\src\\Test_Client\\client.py analyze --size 100000 --method Asynchronous --iteration ${iteration}
I can analyze the corrupted files with a size of 10Mo and with the Asynchronous method and with the iteration ${iteration}
    Run    python C:\\Users\\guillotn\\_work\\git\\poweroffrobot\\src\\Test_Client\\client.py analyze --size 10000000 --method Asynchronous --iteration ${iteration}
I can analyze the corrupted files with a size of 50Mo and with the Asynchronous method and with the iteration ${iteration}
    Run    python C:\\Users\\guillotn\\_work\\git\\poweroffrobot\\src\\Test_Client\\client.py analyze --size 50000000 --method Asynchronous --iteration ${iteration}
I can analyze the corrupted files with a size of 100Mo and with the Asynchronous method and with the iteration ${iteration}
    Run    python C:\\Users\\guillotn\\_work\\git\\poweroffrobot\\src\\Test_Client\\client.py analyze --size 100000000 --method Asynchronous --iteration ${iteration}
# --------------------------------------------------------------------------------------------------------------------- #