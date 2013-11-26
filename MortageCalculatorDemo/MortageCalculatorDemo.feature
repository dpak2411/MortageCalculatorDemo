Feature: MortageCalculatorDemo
	

@mytag
Scenario: publish and run Mortage Calculator
Given alteryx running at" http://devgallery.alteryx.com/"
And I am logged in using "curator@alteryx.com" and "alteryx rocks!"
And I run mortgage calculator with principle 100000 interest 0.04 payments 36
Then I see output 1200
