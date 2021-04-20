package com.example.psu_school

import android.R
import android.app.Activity
import android.content.Context
import android.content.Intent
import android.os.Bundle
import android.view.inputmethod.InputMethodManager
import androidx.appcompat.app.AppCompatActivity
import androidx.appcompat.widget.Toolbar
import androidx.fragment.app.Fragment
import com.example.psu_mobile.UserInfo
import com.example.psu_school.databinding.ActivityMainBinding
import com.example.psu_school.userinterface.fragments.EnterFragment
import com.example.psu_school.userinterface.fragments.NewsFragment
import com.example.psu_school.userinterface.objects.myDrawer
import com.google.zxing.integration.android.IntentIntegrator
import kotlinx.android.synthetic.main.fragment_photo.*
import utilits.replace_fragment


class MainActivity : AppCompatActivity() {
    private lateinit var mBinding: ActivityMainBinding
    private lateinit var mToolBar: Toolbar
    public lateinit var mmyDrawer: myDrawer
    public lateinit var mUserInfo: UserInfo

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        mBinding = ActivityMainBinding.inflate(layoutInflater)
        setContentView(mBinding.root)
        initFieleds()
        initFunc()
    }

    private fun initFunc(){
        setSupportActionBar(mToolBar)
        if(true){
            mmyDrawer.createDrawer()
            replace_fragment(NewsFragment(), false)
        }
        else{
            replace_fragment(EnterFragment(), false)
        }
    }

    private fun initFieleds(){
        mToolBar = mBinding.mainToolBar
        mUserInfo = UserInfo()
        mmyDrawer = myDrawer(this, mToolBar, mUserInfo)
    }

    fun hide_key_board(){
        val imm:InputMethodManager = getSystemService(Context.INPUT_METHOD_SERVICE) as InputMethodManager
        imm.hideSoftInputFromWindow(window.decorView.windowToken, 0)
    }
}