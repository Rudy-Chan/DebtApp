   M S B A M L   `   `   `  ����D  @Wpf_Audit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null%clr-namespace:Wpf_Audit	Wpf_Audit  5          Wpf_Audit.ProbarRotate   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      )localclr-namespace:Wpf_Audit     5        � 	Resources  5	         EllipseStyle �� 
TargetType �� Property �� Value�� 5
   
   %�   &	        �� 5      " A�6)   .�5      ��  � ��$ 16��6*   �� 5       � ��$ 16��6+   �� 5       �@��Stretch$
 Fill=�6,   �� 5       � �� �	#FF2CB6E76)   5      5      .��5      � -   5         �Name$ 
LayoutRoot��5      
������ $��Center=�5      $��Center=�6,   z�5   
   �� 5      -   5        SpinnerScale�� �� ScaleX$	 1.0Q�5   5    �� ScaleY$	 1.0Q�6B   .+�5   
   ��  J� RenderTransformOrigin$ 0.5,0.5&�6   $	��120��62   $	��120��6>   $��Center=�5      $��Center=�6.   .+�5      A� #��[  6    J� Opacity$	 1.0Q�6=   	 �� Left$	 14.64��5      

 �� Top$
 14.64��6,   A� 5      #��[  6   $	 0.8Q�6<   $	 0��5      $
 50��6&   A� 5      #��[  6   $	 0.6Q�6<   $	 14.64��5       $
 85.35��6*   A� 5!      #��[  6   $	 0.4Q�6=   $	 50��5"      $	
 100��6)   A� 5#      #��[  6   $	 0.2Q�6<   $	 85.35��5$      $
 85.35��6*   A� 5%      #��[  6   $	 0.1Q�6<   $		 100��5&      $
 50��6(   z�5'      �� 5(      -   5        SpinnerRotate�� �� Angle$ 0Q�5(   :    � Triggers 5*      :� 5+       :� RoutedEvent$ ContentControl.Loaded��6   .[�5,      �� .��5-      �� .��5.      W�  �� 
TargetName$ SpinnerRotate��5/   %    �� TargetProperty$ (RotateTransform.Angle)�50   %    W� From$ 0X�51   &   	 W� To$	 360X�6/    g� Duration$ 0:0:05D�69    g� RepeatBehavior$ Forever��6K   52      53      54      56      57      58      