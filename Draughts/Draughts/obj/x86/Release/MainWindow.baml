   M S B A M L   `   `   `  ���� C  ?Draughts, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null#clr-namespace:DraughtsDraughts      Draughts.MainWindow   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     (localclr-namespace:Draughts     -     � Title$  Draughts�� � WindowStartupLocation$ CenterScreen=�$	��480��$	��600�� � 
ResizeMode$ 	CanResize=� � Icon$ 	board.jpg�� +� FocusedElement ��  �� Self �� RelativeSource# Z  �� Mode$ OneTime=� J� CommandBindings )  #System.Windows.Input.CommandBinding  -   	   Command$	 local:DataCommands.Delete��  -   $!	 local:DataCommands.AddBlack��  -   $!	 local:DataCommands.AddWhite��  -   $%	 local:DataCommands.AddBlackKing��  -   $%	 local:DataCommands.AddWhiteKing������ ��� 
 � Offset$	
 0.0Q� � Color ��  �� A$	 255�� �� R$ 95�� �� G$ 91�� �� B$	 149��� $ DarkSlateBlue��$	
 0.4Q�.��(� $��0q�$��Stretch=�$��Stretch=� (� Stretch$ Uniform=�.��� -   $��Grid1��$��Stretch=�$��Stretch=� � RowDefinitions �� $��10 ��� $	��400 ��� $��2 � � ColumnDefinitions �� $	��150 ��� $	��400 ��� $��2 �.+�� -   $��	GridBoard��$��1��$��1�� �� $��* ��� $��* ��� $��* ��� $��* ��� $��* ��� $��* ��� $��* ��� $��* � �� $��* ��� $��* ��� $��* ��� $��* ��� $��* ��� $��* ��� $��* ��� $��* ��� -	    ��Name$ canvas��$��1��$��2��
������    context1 � ContextMenu# [  .+��� -
   $��txtGameOver��$��67(�$��	ExtraBold"��� $��0��$��0�� �� Orientation$ 
Horizontal=�$	��Top=�$��2��.+��� -   $��cmdNewB��$��10��$��75��|�.  $	��Top=�.���� $��	Black.jpg��$
��Fill=� � ToolTip _� $��New game: Play black.��  TooltipStyle#��[ �� -   $��cmdNewW��$��10��$��75��$	��Top=�|�.  .���� $��	White.jpg��$
��Fill=� _� $��New game: Play white.��#��[ �� -   $��cmdClear��$��10��$��50��$	��Top=�$y�	Collapsed=�|�.  .���� $��	Board.jpg��$
��Fill=� _� $��Clear board.��#��[ �� $��1��$��0��$ 
Horizontal=�$��Right=�.+��� -   $��cmdSetup��$��10��$��50��$y�Hidden=�
���"�"�$	��Top=�|�.   _� $��Toggle Setup mode��#��[ �� $��1��$ Vertical=�$��Center=�.+��� -   $��
txtPlaying��$��1��$��14(�
�������$��Center=��� -   $��cmdDraw��  GlassButton#��[ $��0��$��0��$y�Visible=�.��Accept Draw�� -   $��cmdMove��$��1��$��35��$��35��$��1q�$��Right=�$y�Hidden=�|�.  .��� -   $��polyPlay��
���   � � Points �    $�  z  $�  _� $��Play next move.��#��[ �� -   $��	cmdReturn��$��1��$��35��$��35��$��1q�$
��Left=�$y�Hidden=�|�.  
������ .���� $��
return.jpg��$��Stretch=�$��Stretch=�