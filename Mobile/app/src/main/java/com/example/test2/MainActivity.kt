package com.example.test2

import android.app.Activity
import android.content.Context
import android.content.Intent
import android.os.Bundle
import android.os.StrictMode
import android.view.inputmethod.InputMethodManager
import androidx.appcompat.app.AppCompatActivity
import androidx.appcompat.widget.Toolbar
import com.example.test2.databinding.ActivityMainBinding
import com.example.test2.userinterface.fragments.BaseFragment
import com.example.test2.userinterface.fragments.MailFragment
import com.example.test2.userinterface.fragments.NewsFragment
import com.example.test2.userinterface.objects.RegisterActivity
import com.example.test2.userinterface.objects.myDrawer
import com.squareup.moshi.Moshi
import com.squareup.moshi.kotlin.reflect.KotlinJsonAdapterFactory
import kotlinx.android.synthetic.main.fragment_mail.*
import utilits.replace_activity
import utilits.replace_fragment
import java.io.ByteArrayInputStream
import java.net.URL
import kotlin.random.Random

class MainActivity : AppCompatActivity() {
    private lateinit var mBinding: ActivityMainBinding
    private lateinit var mToolBar: Toolbar
    public lateinit var mmyDrawer: myDrawer

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        mBinding = ActivityMainBinding.inflate(layoutInflater)
        setContentView(mBinding.root)
        initFieleds()
        initFunc()
    }

    private fun initFunc(){
        if(true){
            setSupportActionBar(mToolBar)
            mmyDrawer.createDrawer()
            replace_fragment(NewsFragment())
        }
        else{
            replace_activity(RegisterActivity())
        }

    }

    private fun initFieleds(){
        mToolBar = mBinding.mainToolBar
        mmyDrawer = myDrawer(this, mToolBar)
    }

    fun hide_key_board(){
        val imm:InputMethodManager = getSystemService(Context.INPUT_METHOD_SERVICE) as InputMethodManager
        imm.hideSoftInputFromWindow(window.decorView.windowToken, 0)
    }
}