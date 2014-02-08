Imports Oracle.DataAccess.Client
Imports Oracle.DataAccess.Types
Public Class Form1
    'Dim dt As DataTable
    Public tgl As String
    Public no_urut As String
    Public txtNpk As String

    Sub Load_noUrut()
        cmd = New OracleCommand("select no_urut from oto_gaji", con)
        dr = cmd.ExecuteReader
        While dr.Read
            ComboBox2.Items.Add(dr("no_urut"))
        End While
    End Sub

    Sub Load_tgl()
        cmd = New OracleCommand("select TO_CHAR(TGL,'fmDD-MON-YYYY') AS TGL from uhadir order by no_urut asc", con)
        dr = cmd.ExecuteReader
        While dr.Read
            ComboBox1.Items.Add(dr("TGL"))
        End While
    End Sub

    Sub cetak_otoGajiAll()
        con.Close()
        da = New OracleDataAdapter("select n_buat as Nama_Pembuat,j_buat as Jabatan_Pembuat," &
                                   "n_periksa as Diperiksa_Oleh,j_periksa as Jabatan_pemeriksa,n_setuju as Disetujui_Oleh," &
                                   "j_setuju as Jabatan_Penyetuju,n_periksa1 as Diperiksa_ACC1,j_periksa1 as Jabatan_Pemeriksa_ACC1," &
                                   "n_periksa2 as Diperiksa_ACC2,j_periksa2 Jabatan_Pemeriksa_ACC2 from oto_gaji", con)
        ds = New DataSet
        'dt = New DataTable
        da.Fill(ds, "oto_gaji")
        DataGridView2.DataSource = ds.Tables("oto_gaji")
        DataGridView2.ReadOnly = True

    End Sub

    Sub cetak_otoGaji()
        no_urut = ComboBox2.SelectedItem.ToString()
        con.Close()
        da = New OracleDataAdapter("select n_buat as Nama_Pembuat,j_buat as Jabatan_Pembuat," &
                                   "n_periksa as Diperiksa_Oleh,j_periksa as Jabatan_pemeriksa,n_setuju as Disetujui_Oleh," &
                                   "j_setuju as Jabatan_Penyetuju,n_periksa1 as Diperiksa_ACC1,j_periksa1 as Jabatan_Pemeriksa_ACC1," &
                                   "n_periksa2 as Diperiksa_ACC2,j_periksa2 Jabatan_Pemeriksa_ACC2 from oto_gaji where no_urut=" + no_urut + "", con)
        ds = New DataSet
        da.Fill(ds, "oto_gaji")
        'TextBox1.Text = ds.Tables(0).Rows(1).Item(1)
        DataGridView2.DataSource = ds.Tables("oto_gaji")
        DataGridView2.ReadOnly = True
    End Sub

    Sub cetaksemua()
        con.Close()
        da = New OracleDataAdapter("select TGL,NILAI,STATUS from uhadir", con)
        ds = New DataSet
        da.Fill(ds, "uhadir")
        DataGridView1.DataSource = ds.Tables("uhadir")
        DataGridView1.ReadOnly = True
    End Sub

    Sub cetaktgl()
        tgl = ComboBox1.SelectedItem.ToString()
        If tgl = "All" Then
            tgl = ""
        End If
        con.Close()
        da = New OracleDataAdapter("select TGL,NILAI from uhadir where TGL='" + tgl + "'", con)
        ds = New DataSet
        da.Fill(ds, "uhadir")
        DataGridView1.DataSource = ds.Tables("uhadir")
        DataGridView1.ReadOnly = True
    End Sub

    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Call koneksi()
        Call cetaksemua()
        con.Open()
        Call Load_noUrut()
        Call Load_tgl()
        Call loadNpk()
        Call CariGajiTxt()

    End Sub

    Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button2.Click
        tgl = ComboBox1.SelectedItem.ToString()
        Call koneksi()
        If tgl = "All" Then
            If (MsgBox("Lanjutkan Cetak Data?", vbYesNoCancel) = MsgBoxResult.Yes) Then
                Call cetaksemua()
                Form2.Show()
            End If
        ElseIf tgl = "" Then
            MsgBox("Pilih Terlebih Dahulu")
        Else
            If (MsgBox("Lanjutkan Cetak Data " + tgl + "?", vbYesNoCancel) = MsgBoxResult.Yes) Then
                Call cetaktgl()
                Form2.Show()
            End If
        End If
        
    End Sub

    Private Sub ComboBox2_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ComboBox2.SelectedIndexChanged
        Call koneksi()
        no_urut = ComboBox2.SelectedItem.ToString()

        If no_urut = "All" Then
            Call cetak_otoGajiAll()
        Else
            Call cetak_otoGaji()
        End If

    End Sub

    Private Sub ComboBox1_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ComboBox1.SelectedIndexChanged
        Call koneksi()
        tgl = ComboBox1.SelectedItem.ToString()
        If tgl = "All" Then
            Call cetaksemua()
        Else
            Call cetaktgl()
        End If

    End Sub

    Private Sub Button3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button3.Click
        Call koneksi()
        no_urut = ComboBox2.SelectedItem.ToString()

        If no_urut = "All" Then
            Call cetak_otoGajiAll()
            Form3.Show()
        Else
            Call cetak_otoGaji()
            Form3.Show()
        End If
    End Sub

    Sub CariGajiTxt()
        ComboBox3.Text = "All"
        txtNpk = TextBox1.Text.ToUpper
        con.Close()
        da = New OracleDataAdapter("SELECT A.NPK, C.TGL AS TANGGAL_BERLAKU, A.NAMA,D.N_JABATAN as JABATAN, B.NAMA_DEPARTEMEN AS DEPARTEMEN," &
                                   " C.NILAI, C.TUNJ_T AS TUNJANGAN FROM ((KARYAWAN A JOIN DEPT B ON A.DEPT=B.ID_DEPARTEMEN)" &
                                   " JOIN GAPOK C ON A.NPK=C.NPK) JOIN JAB D ON A.JAB=D.ID_JABATAN where A.NAMA like '%" + txtNpk + "%'", con)
        ds = New DataSet
        da.Fill(ds, "gapok")
        DataGridView3.DataSource = ds.Tables("gapok")
        DataGridView3.ReadOnly = True
    End Sub

    Sub CariGajicombo()
        TextBox1.Text = ""
        txtNpk = ComboBox3.SelectedItem.ToString()
        con.Close()
        If txtNpk = "All" Then
            da = New OracleDataAdapter("SELECT A.NPK, C.TGL AS TANGGAL_BERLAKU, A.NAMA,D.N_JABATAN as JABATAN, B.NAMA_DEPARTEMEN AS DEPARTEMEN," &
                                   " C.NILAI, C.TUNJ_T AS TUNJANGAN FROM ((KARYAWAN A JOIN DEPT B ON A.DEPT=B.ID_DEPARTEMEN)" &
                                   " JOIN GAPOK C ON A.NPK=C.NPK) JOIN JAB D ON A.JAB=D.ID_JABATAN", con)
            ds = New DataSet
        Else
            da = New OracleDataAdapter("SELECT A.NPK, C.TGL AS TANGGAL_BERLAKU, A.NAMA,D.N_JABATAN as JABATAN, B.NAMA_DEPARTEMEN AS DEPARTEMEN," &
                                   " C.NILAI, C.TUNJ_T AS TUNJANGAN FROM ((KARYAWAN A JOIN DEPT B ON A.DEPT=B.ID_DEPARTEMEN)" &
                                   " JOIN GAPOK C ON A.NPK=C.NPK) JOIN JAB D ON A.JAB=D.ID_JABATAN where A.NPK like '%" + txtNpk + "%'", con)
            ds = New DataSet
        End If
        
        da.Fill(ds, "gapok")
        DataGridView3.DataSource = ds.Tables("gapok")
        DataGridView3.ReadOnly = True
    End Sub

    Sub loadNpk()
        cmd = New OracleCommand("select npk from gapok where status='ON' order by npk asc", con)
        dr = cmd.ExecuteReader
        While dr.Read
            ComboBox3.Items.Add(dr("npk"))
        End While
    End Sub

    Private Sub TextBox1_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TextBox1.TextChanged
        Call koneksi()
        Call CariGajiTxt()
    End Sub

    Private Sub ComboBox3_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ComboBox3.SelectedIndexChanged
        Call koneksi()
        Call CariGajicombo()
    End Sub

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        Call koneksi()
        Form4.Show()
    End Sub

End Class
