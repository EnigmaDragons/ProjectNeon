Standart UI Window

Window for UGUI. It have standart move and change size mechanic. 
In window you can put output in camera(fast variant). Besides 
supported by 3D UI. Your gamers can customize interface and control
(buttons, joystics and other elements) by touchscreen.

Demo Scenes:

	/AlchemistUITools/StandartWindow/windowScene general demonstrantion
3D window and camera view in window.
	/AlchemistUITools/StandartWindow/ruleSettings demonstrantion customization
control by touchscreen.
	/AlchemistUITools/StandartWindow/game demonstrantion result customization
control by touchscreen.


Scripts:

	WindowLogic - main class for window logic. It implements move and change sizes.
	RuleElemLogic - child WindowLogic. It implements save relative(without 
distortion size proportion) position and size in PlayerPrefs.
	GameRuleElem - read from PlayerPrefs position and size and set RectTransform
size and position(!!!!!!!RectTransform can't have stretchy coordinates!!!!!).
	ViewOnRect - implements pivot camera output and RectTransform.
	UIScreenTool - set functions implements convertation coordinates(world, canvas and screen)
	Rotate - rotate GameObject
	