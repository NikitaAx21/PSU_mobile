package com.example.psu_school.userinterface.fragments

import android.app.Activity
import android.content.Context
import android.content.Intent
import android.net.Uri
import com.example.psu_school.R
import kotlinx.android.synthetic.main.fragment_mail.*
import utilits.getFilenameFromUri
import utilits.show_toast


@Suppress("DEPRECATION")
class MailFragment : BaseFragment(R.layout.fragment_mail) {

    private var mContext: Context? = null
    var mUri: Uri? = null
    lateinit var mfileName: String

    override fun onStart() {
        super.onStart()
        add_mail_file_button.setOnClickListener{add_file()}
        send_mail_button.setOnClickListener{send_mail_fun()}
    }

    override fun onAttach(context: Context) {
        super.onAttach(context)
        mContext = context
    }

    private fun add_file(){
        val intent = Intent().setType("*/*").setAction(Intent.ACTION_GET_CONTENT)
        startActivityForResult(Intent.createChooser(intent, getString(R.string.add_file_text)), 111)
    }

    override fun onActivityResult(requestCode: Int, resultCode: Int, data: Intent?) {
        super.onActivityResult(requestCode, resultCode, data)
        if(data != null && requestCode == 111 && resultCode == Activity.RESULT_OK){
            mUri = data.data
            mfileName = getFilenameFromUri(mUri!!)
            mail_file_name.setText(mfileName)
        }
        else show_toast(getString(R.string.mail_file_err))
    }

    private fun send_mail_fun() {
        try {
            val intent = Intent(Intent.ACTION_SEND)
            if(mail_file_name.text.isNotEmpty()){
                intent.setDataAndType(mUri!!, mContext!!.contentResolver.getType(mUri!!))
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