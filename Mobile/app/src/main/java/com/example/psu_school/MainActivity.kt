package com.example.psu_school

import android.content.Context
import android.os.Bundle
import android.view.inputmethod.InputMethodManager
import androidx.appcompat.app.AppCompatActivity
import androidx.appcompat.widget.Toolbar
import com.example.psu_mobile.UserInfo
import com.example.psu_school.databinding.ActivityMainBinding
import com.example.psu_school.userinterface.fragments.NewsFragment
import com.example.psu_school.userinterface.objects.RegisterActivity
import com.example.psu_school.userinterface.objects.myDrawer
import kotlinx.android.synthetic.main.fragment_mail.*
import utilits.replace_activity
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