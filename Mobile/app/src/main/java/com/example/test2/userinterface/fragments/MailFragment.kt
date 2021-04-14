package com.example.test2.userinterface.fragments

import android.R.attr
import android.app.Activity
import android.content.Intent
import android.net.Uri
import android.os.Environment
import com.example.test2.R
import kotlinx.android.synthetic.main.fragment_mail.*
import kotlinx.android.synthetic.main.fragment_news.*
import utilits.getFilenameFromUri
import utilits.show_toast
import java.io.File


@Suppress("DEPRECATION")
class MailFragment : BaseFragment(R.layout.fragment_mail) {

    private var mSmoothScrollToPosition = true
    lateinit var filename: String

    override fun onStart() {
        super.onStart()
        add_mail_file_button.setOnClickListener{add_file()}
        send_mail_button.setOnClickListener{send_mail()}
    }

    private fun add_file(){
        val intent = Intent().setType("*/*").setAction(Intent.ACTION_GET_CONTENT)
        startActivityForResult(Intent.createChooser(intent, getString(R.string.add_file_text)), 111)
    }

    override fun onActivityResult(requestCode: Int, resultCode: Int, data: Intent?) {
        super.onActivityResult(requestCode, resultCode, data)
        if(data != null && requestCode == 111 && resultCode == Activity.RESULT_OK){
            val uri = data.data
            filename = getFilenameFromUri(uri!!)
            mail_file_name.setText(filename)
        }
        else show_toast(getString(R.string.mail_file_err))
    }

    private fun send_mail() {
        if(mail_send_to_input.text.toString().isEmpty()){
            show_toast(getString(R.string.mail_send_to_err))
        }
        else if(mail_subject_input.text.toString().isEmpty()){
            show_toast(getString(R.string.mail_subject_err))
        }
        else if(mail_message_input.text.toString().isEmpty()){
            show_toast(getString(R.string.mail_message_err))
        }
        else{
            send_mail_fun()
        }
    }

    private fun send_mail_fun(){
        val mails = mail_send_to_input.text.split(",").toTypedArray()
        val intent = Intent(Intent.ACTION_SEND)
        /*if(!filename.isEmpty()){
            val file_location = File(Environment.getExternalStorageDirectory().getAbsolutePath(), filename)
            val path = Uri.fromFile(file_location)

        }*/
        intent.type = "text/plain"
        intent.putExtra(Intent.EXTRA_EMAIL, mails)
        intent.putExtra(Intent.EXTRA_SUBJECT, mail_subject_input.text.toString()) //тема
        intent.putExtra(Intent.EXTRA_TEXT, mail_message_input.text.toString()) //Сообщение
        //intent.putExtra(Intent.EXTRA_STREAM, path) //Сообщение
        try {
            startActivity(Intent.createChooser(intent, getString(R.string.mail_chooser_text)))
        } catch (ex: Exception) {
            show_toast(getString(R.string.mail_chooser_err))
        }
    }
}