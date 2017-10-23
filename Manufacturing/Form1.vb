Imports System.Data.SqlClient
Public Class Form1
    Dim errCount As Integer = 0
    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.ActiveControl = txtSO
        txtSO.SelectionStart = 0
        txtSO.Focus()
        Dim errCount As Integer = 0
        Dim formWidth = Me.Size.Width()
        Dim formHeight = Me.Size.Height()
        Label4.Location = New Point(formWidth - 355, formHeight - 70)


        Label11.Text = "Initializing"
        txtValid.Text = "Initializing"


    End Sub

    Private Sub btnInput_Click(sender As Object, e As EventArgs) Handles btnInput.Click

        '''''''VARIABLES'''''''
        Dim con As SqlConnection
        Dim com As SqlCommand
        Dim com2 As SqlCommand = New SqlCommand

        'LOCALHOST'
        'con = New SqlConnection("Data Source=localhost;Initial Catalog=SomnoMed;User ID=SOMNOMED-IBM-Guest;Password=Somnomed01")
        'LOCALHOST'



        'SERVER'
        con = New SqlConnection("Data Source=SOMNOMED-IBM;Initial Catalog=SomnoMed;User ID=SOMNOMED-IBM-Guest;Password=Somnomed01")
        'SERVER'
        com = New SqlCommand

        '''''''QUERY FOR SELECTING PROCESS OF THE PRODUCT'''''''''''
        Dim prodquery As String = " SELECT Process_ID, Process_Name, Stage_ID FROM Process WHERE Process_ID LIKE '@PI'"
        Dim prodcmd As SqlCommand = New SqlCommand(prodquery, con)
        prodcmd.Parameters.AddWithValue("@PI", txtProcess.Text)
        '''''''QUERY FOR SELECTING PROCESS OF THE PRODUCT'''''''''''

        '''''''QUERY FOR ASSURING IF DATA WAS ALREADY ENTERED OR NOT'''''''''''
        Dim thequery As String = "SELECT  [Dispatch Invoice Number], Employee_ID, Time_In, Time_Out, Date, Status, Process_ID, Process_Name, Stage_ID " &
        " FROM Production WHERE [Dispatch Invoice Number] = @PS AND Process_ID LIKE '@PD'"

        Dim socmd As SqlCommand = New SqlCommand(thequery, con)
        socmd.Parameters.AddWithValue("@PS", txtSO.Text)
        socmd.Parameters.AddWithValue("@PD", txtProcess.Text)
        '''''''QUERY FOR ASSURING IF DATA WAS ALREADY ENTERED OR NOT'''''''''''


        '''''''''''''Start of Code'''''''''''''''''''''
        Try

            If Len(txtID.Text) < 9 Or Len(txtSO.Text) < 5 Or Len(txtSO.Text) = 7 Or Len(txtProcess.Text) < 18 Then

                Invalid()

                txtSO.Text = ""
                txtID.Text = ""
                txtProcess.Text = ""
                txtSO.Focus()

            Else


                con.Open()
                Using reader As SqlDataReader = socmd.ExecuteReader()

                    reader.Read()

                    If reader.HasRows Then

                        Dim insert As String = "UPDATE Production SET Time_Out = @TO WHERE [Dispatch Invoice Number] = @PS AND Process_ID = @PD"
                        Dim cmd2 As SqlCommand = New SqlCommand(insert, con)

                        cmd2.Parameters.AddWithValue("@PS", txtSO.Text)
                        cmd2.Parameters.AddWithValue("@PD", txtProcess.Text)
                        cmd2.Parameters.AddWithValue("@TO", String.Format("{0:hh:mm:ss tt}", System.DateTime.Now))
                        reader.Close()

                        cmd2.ExecuteNonQuery()
                        Success()
                    Else

                        reader.Close()
                        com2.Connection = con
                        com2.CommandText = "INSERT INTO Production (Employee_ID,[Dispatch Invoice Number], Time_In, Date, Status, Process_ID, Process_Name) Values (@ED, @PS, @TI, @DA, @ST, @PD, @PN)"
                        com2.Parameters.AddWithValue("@ED", txtID.Text)
                        com2.Parameters.AddWithValue("@PS", txtSO.Text)
                        com2.Parameters.AddWithValue("@TI", String.Format("{0:hh:mm:ss tt}", System.DateTime.Now))
                        com2.Parameters.AddWithValue("@DA", Format(Date.Now, "yyyy-MM-dd"))
                        com2.Parameters.AddWithValue("@ST", "On Process")
                        com2.Parameters.AddWithValue("@PD", txtProcess.Text)
                        com2.Parameters.AddWithValue("@PN", Label14.Text)
                        com2.ExecuteNonQuery()
                        Success()
                        '
                        'AFTER INPUT OF EMPLOYEE ID, SO, TIME IN, DATE, STATUS AND PROCESS
                        'DETERMINE WHAT PRODUCT IT IS IN SOMTRACK
                        ''


                        'SELECT THE PRODUCT TYPE AND PROCESS ID 
                        'example:
                        ''SELECT DISTINCT r.Product_Type, r.Process_ID
                        ''FROM  Reference r, Process p, Product pr
                        ''WHERE r.Product_Type = 'Flex'
                        ''ORDER BY Process_ID
                        '
                        'IF PROCESS IS NOT INCLUDED IN THE DB
                        'DELETE THE SO
                        'OTHERWISE CONTINUE PROCESS
                        '
                        '
                        Using prodreader As SqlDataReader = prodcmd.ExecuteReader()
                            prodreader.Read()

                            If prodreader.HasRows Then

                                Dim update As String = "UPDATE Production SET Process_Name  = @PN, Stage_ID = @SI " &
                                " WHERE Process_ID = @PD AND [Dispatch Invoice Number] = @DV "
                                Dim pro As SqlCommand = New SqlCommand(update, con)
                                pro.Parameters.AddWithValue("@PD", txtProcess.Text)
                                pro.Parameters.AddWithValue("@PN", prodreader.GetString(1))
                                pro.Parameters.AddWithValue("@SI", prodreader.GetString(2))
                                pro.Parameters.AddWithValue("@DV", txtSO.Text)
                                prodreader.Close()
                                pro.ExecuteNonQuery()
                                Success()

                            End If


                        End Using
                    End If
                End Using
            End If


        Catch ex As Exception

            MessageBox.Show("Error!", ex.InnerException.ToString)

        End Try

        con.Close()

        txtSO.Text = ""
        txtID.Text = ""
        txtProcess.Text = ""
        txtSO.SelectionStart = 0
        txtSO.Focus()

    End Sub

    Private Sub txtSO_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txtSO.KeyDown
        If e.KeyCode = Keys.Enter Then
            Dim con As SqlConnection
            Dim com As SqlCommand



            'LOCALHOST'
            'con = New SqlConnection("Data Source=localhost;Initial Catalog=SomnoMed;User ID=SOMNOMED-IBM-Guest;Password=Somnomed01")
            'LOCALHOST'

            'SERVER'
            con = New SqlConnection("Data Source=10.130.15.40;Initial Catalog=somtrackdbprod;User ID=somtrack2;Password=sompass12345")
            'SERVER'
            com = New SqlCommand

            '''''''QUERY FOR SELECTING PROCESS OF THE PRODUCT'''''''''''
            Dim prodquery As String = "Select LDT.DeviceTypeName as DeviceName From LstDevice as LD Left Join LstDeviceType as LDT ON LD.ProductTypeID = LDT.DeviceTypeId WHERE LD.DeviceID = @prod"
            Dim prodcmd As SqlCommand = New SqlCommand(prodquery, con)
            prodcmd.Parameters.AddWithValue("@prod", txtSO.Text)
            con.Open()

            If (con.State = ConnectionState.Open) Then
            End If

            Using reader As SqlDataReader = prodcmd.ExecuteReader()
                If reader.HasRows Then
                    While reader.Read()
                        Label5.Text = reader.Item("DeviceName")

                        txtValid.Text = "Processing"
                        Label11.Text = "Active"
                        txtID.Focus()
                        errCount = 0

                    End While

                Else
                    txtValid.Text = "Invalid SomTrack"
                    Label11.Text = "Active"
                    Label5.Text = ""
                    txtSO.Text = ""
                    Timer4.Enabled = True
                    Timer3.Enabled = True
                End If
            End Using


            con.Close()

        End If

    End Sub

    Private Sub txtID_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txtID.KeyDown
        If e.KeyCode = Keys.Enter Then
            Dim con As SqlConnection
            Dim com As SqlCommand
            Dim com2 As SqlCommand = New SqlCommand



            'LOCALHOST'
            'con = New SqlConnection("Data Source=localhost;Initial Catalog=SomnoMed;User ID=SOMNOMED-IBM-Guest;Password=Somnomed01")
            'LOCALHOST'

            'SERVER'
            con = New SqlConnection("Data Source=SOMNOMED-IBM;Initial Catalog=SomnoMed;User ID=SOMNOMED-IBM-Guest;Password=Somnomed01")
            'SERVER'
            com = New SqlCommand

            '''''''QUERY FOR SELECTING PROCESS OF THE PRODUCT'''''''''''
            Dim empquery As String = "SELECT CONCAT([Employee_Lname], ', ',[Employee_Fname],' ', [Employee_Mname], '.') as Employee_Name FROM Employee WHERE Employee_ID LIKE @emp"
            Dim empcmd As SqlCommand = New SqlCommand(empquery, con)
            empcmd.Parameters.AddWithValue("@emp", txtID.Text)

            con.Open()
            Using reader As SqlDataReader = empcmd.ExecuteReader()
                If reader.HasRows Then
                    While reader.Read()
                        If (reader.Item("Employee_Name") <> ",  .") Then
                            Label13.Text = reader.Item("Employee_Name")

                            txtProcess.Focus()

                        End If

                    End While
                End If
            End Using

            If (Label13.Text = "") Then
                errCount = errCount + 1
                txtValid.Text = "Invalid Employee ID"
                txtID.Text = ""
                Label11.Text = "Active"
                Label13.Text = ""
                Timer4.Enabled = True
            End If

            If (errCount = 3) Then
                Label5.Text = ""
                Label13.Text = ""
                Label14.Text = ""
                txtSO.Text = ""
                txtID.Text = ""
                txtProcess.Text = ""
                txtSO.Focus()
                Timer3.Enabled = True
                errCount = 0
            End If

            con.Close()

        End If

    End Sub
    Private Sub txtProcess_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txtProcess.KeyDown
        If e.KeyCode = Keys.Enter Then
            Dim con As SqlConnection
            Dim com As SqlCommand
            Dim com2 As SqlCommand = New SqlCommand



            'LOCALHOST'
            'con = New SqlConnection("Data Source=localhost;Initial Catalog=SomnoMed;User ID=SOMNOMED-IBM-Guest;Password=Somnomed01")
            'LOCALHOST'

            'SERVER'
            con = New SqlConnection("Data Source=SOMNOMED-IBM;Initial Catalog=SomnoMed;User ID=SOMNOMED-IBM-Guest;Password=Somnomed01")
            'SERVER'
            com = New SqlCommand

            '''''''QUERY FOR SELECTING PROCESS OF THE PRODUCT'''''''''''
            Dim procquery As String = "SELECT Process_Name FROM Process WHERE Process_ID LIKE @proc"
            Dim proccmd As SqlCommand = New SqlCommand(procquery, con)
            proccmd.Parameters.AddWithValue("@proc", txtProcess.Text)


            con.Open()

            Using reader As SqlDataReader = proccmd.ExecuteReader()
                If reader.HasRows Then
                    While reader.Read()
                        Label14.Text = reader.Item("Process_Name")

                        btnInput.PerformClick()
                    End While

                End If
            End Using

            If (txtValid.Text <> "Record Saved") Then
                errCount = errCount + 1
                txtValid.Text = "Invalid Process"
                Label11.Text = "Active"
                Timer4.Enabled = True
            End If

            If (errCount = 3) Then
                Label5.Text = ""
                Label13.Text = ""
                Label14.Text = ""
                txtSO.Text = ""
                txtID.Text = ""
                txtProcess.Text = ""
                txtSO.Focus()
                Timer3.Enabled = True
                errCount = 0
            End If

            con.Close()

        End If
    End Sub

    Private Sub Success()
        txtValid.Text = "Record Saved"
        Label5.Text = ""
        Label13.Text = ""
        Label14.Text = ""
        txtSO.Text = ""
        txtID.Text = ""
        txtProcess.Text = ""
        txtSO.Focus()
        Timer3.Enabled = True

    End Sub

    Private Sub Invalid()
        txtValid.Text = "Saving Failed"
        Label5.Text = ""
        Label13.Text = ""
        Label14.Text = ""
        txtSO.Text = ""
        txtID.Text = ""
        txtProcess.Text = ""
        txtSO.Focus()
        Timer3.Enabled = True
        Beep()
    End Sub

    Private Sub txtSO_MaskInputRejected(sender As Object, e As MaskInputRejectedEventArgs) Handles txtSO.MaskInputRejected

    End Sub

    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
        Dim con As SqlConnection

        'AUS'
        'con = New SqlConnection("Data Source=10.130.15.40;Initial Catalog=somtrackdbprod;User ID=somtrack2;Password=sompass12345")
        'LOCALHOST'

        'Local SERVER'
        con = New SqlConnection("Data Source=SOMNOMED-IBM;Initial Catalog=SomnoMed;User ID=SOMNOMED-IBM-Guest;Password=Somnomed01")

        Try
            con.Open()
            Me.Enabled = True
            Label7.ForeColor = Color.Green
            Label7.Text = "Online"
            If (Label5.Text = "") Then
                txtValid.Text = "Waiting"
                Label11.Text = "Idle"
            End If
            con.Close()

        Catch ex As Exception
            Label7.Text = "Offline"
            txtValid.Text = "Disabled"
            Label11.Text = "Disabled"
            Label7.ForeColor = Color.Red
            'disable form
            Me.Enabled = False
            Timer1.Enabled = True

        End Try

        Dim con2 As SqlConnection
        con2 = New SqlConnection("Data Source=10.130.15.40;Initial Catalog=somtrackdbprod;User ID=somtrack2;Password=sompass12345")

        Try
            con2.Open()
            Me.Enabled = True
            Label23.ForeColor = Color.Green
            Label23.Text = "Online"
            If (Label5.Text = "") Then
                txtValid.Text = "Waiting"
                Label11.Text = "Idle"
            End If
            con2.Close()

        Catch ex As Exception
            Label23.Text = "Offline"
            txtValid.Text = "Disabled"
            Label11.Text = "Disabled"
            Label23.ForeColor = Color.Red
            'disable form
            Me.Enabled = False
            Timer1.Enabled = True
        End Try



    End Sub

    Private Sub txtID_MaskInputRejected(sender As Object, e As MaskInputRejectedEventArgs) Handles txtID.MaskInputRejected

    End Sub
    Private Sub txtProcess_MaskInputRejected(sender As Object, e As MaskInputRejectedEventArgs) Handles txtProcess.MaskInputRejected

    End Sub


    Private Sub Timer2_Tick(sender As Object, e As EventArgs) Handles Timer2.Tick

        Dim current As DateTime
        current = DateTime.Now

        Dim ausTime = System.TimeZoneInfo.ConvertTime(current, TimeZoneInfo.FindSystemTimeZoneById("Cen. Australia Standard Time"))
        ausTime = ausTime.ToString("hh:mm:ss  tt")


        Dim CurrentDateTime As DateTime
        CurrentDateTime = DateTime.Now
        Dim phTime = CurrentDateTime.ToString("hh:mm:ss  tt")



        Label25.Text = ausTime


        Label9.Text = phTime
    End Sub

    Private Sub Timer3_Tick(sender As Object, e As EventArgs) Handles Timer3.Tick
        If (Label5.Text = "") Then
            Label11.Text = "Idle"
            Timer3.Enabled = False
        Else
            Label11.Text = "Active"
        End If

    End Sub

    Private Sub Timer4_Tick(sender As Object, e As EventArgs) Handles Timer4.Tick
        txtValid.Text = "Waiting"
        Timer4.Enabled = False
    End Sub

End Class