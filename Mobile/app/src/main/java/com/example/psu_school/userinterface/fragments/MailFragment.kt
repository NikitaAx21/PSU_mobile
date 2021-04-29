package com.example.psu_school.userinterface.fragments

import android.app.Activity
import android.content.Context
import android.content.Intent
import android.net.Uri
import androidx.fragment.app.Fragment
import com.example.psu_school.R
import com.example.psu_school.utilits.MAIL_REQUEST_CODE
import com.example.psu_school.utilits.MAIN_ACTIVITY
import kotlinx.android.synthetic.main.fragment_mail.*
import utilits.*


@Suppress("DEPRECATION")
class MailFragment : Fragment(R.layout.fragment_mail) {

    private var mUri: Uri? = null
    private lateinit var mFileName: String

    override fun onStart() {
        super.onStart()
        add_mail_file_button.setOnClickListener { add_file() }
        send_mail_button.setOnClickListener { send_mail_fun() }
    }

    override fun onStop() {
        super.onStop()
        hide_key_board()
    }

    override fun onResume() {
        super.onResume()
        set_toolbar_info(getString(R.string.drawer_mail_name))
    }

    override fun onPause() {
        super.onPause()
        unset_toolbar_info()
    }

    override fun onAttach(context: Context) {
        super.onAttach(context)
    }

    private fun add_file() {
        val intent = Intent(Intent.ACTION_GET_CONTENT)
        intent.type = "*/*"
        startActivityForResult(Intent.createChooser(intent, getString(R.string.add_file_text)), MAIL_REQUEST_CODE)
    }

    override fun onActivityResult(requestCode: Int, resultCode: Int, data: Intent?) {
        super.onActivityResult(requestCode, resultCode, data)
        if (data != null && requestCode == MAIL_REQUEST_CODE && resultCode == Activity.RESULT_OK) {
            mUri = data.data
            mFileName = get_filename_from_uri(mUri!!)
            mail_file_name.text = mFileName
        } else show_toast(getString(R.string.mail_file_err))
    }

    private fun send_mail_fun() {
        try {
            val intent = Intent(Intent.ACTION_SEND)
            if (mail_file_name.text.isNotEmpty()) {
                intent.setDataAndType(mUri!!, MAIN_ACTIVITY.contentResolver.getType(mUri!!))
                intent.putExtra(Intent.EXTRA_STREAM, mUri)
                intent.addFlags(Intent.FLAG_GRANT_READ_URI_PERMISSION)
                intent.addFlags(Intent.FLAG_GRANT_WRITE_URI_PERMISSION)
            }
            val mails = mail_send_to_input.text.split(",").toTypedArray()
            intent.putExtra(Intent.EXTRA_EMAIL, mails)//электронные адреса
            intent.putExtra(Intent.EXTRA_SUBJECT, mail_subject_input.text.toString()) //тема
            intent.putExtra(Intent.EXTRA_TEXT, mail_message_input.text.toString()) //Сообщение
            startActivity(Intent.createChooser(intent, getString(R.string.mail_chooser_text)))
        } catch (ex: Exception) {
            show_toast(getString(R.string.mail_chooser_err))
        }
    }
}