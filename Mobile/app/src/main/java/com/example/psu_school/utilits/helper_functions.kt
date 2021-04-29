@file:Suppress("DEPRECATION")

package utilits

import android.content.Intent
import android.net.Uri
import android.provider.OpenableColumns
import android.view.View
import android.view.inputmethod.InputMethodManager
import android.widget.EditText
import android.widget.ImageView
import android.widget.Toast
import androidx.appcompat.app.AppCompatActivity
import androidx.fragment.app.Fragment
import com.example.psu_school.MainActivity
import com.example.psu_school.R
import com.example.psu_school.models.this_user
import com.example.psu_school.utilits.MAIN_ACTIVITY
import com.squareup.picasso.Picasso
import kotlinx.android.synthetic.main.activity_main.*
import kotlinx.android.synthetic.main.activity_main.view.*
import kotlinx.android.synthetic.main.toolbar_info.view.*


//показывает сообщение во фрагментах
fun Fragment.show_toast(message: String) {
    Toast.makeText(MAIN_ACTIVITY, message, Toast.LENGTH_SHORT).show()
}

fun Fragment.set_toolbar_info(name: String) {
    MAIN_ACTIVITY.mainToolBar.toolbar_info.visibility = View.VISIBLE
    MAIN_ACTIVITY.mainToolBar.toolbar_info
        .toolbar_info_text.text = name
}

fun Fragment.unset_toolbar_info() {
    (activity as MainActivity).mainToolBar.toolbar_info.visibility = View.GONE
}

fun AppCompatActivity.show_toast(message: String) {
    Toast.makeText(this, message, Toast.LENGTH_SHORT).show()
}

//функция для переключения активити
fun AppCompatActivity.replace_activity(activity: AppCompatActivity) {
    val register = Intent(this, activity::class.java)
    startActivity(register)
}

fun AppCompatActivity.replace_activity_with_data(activity: AppCompatActivity) {
    val register = Intent(this, activity::class.java)
    startActivity(register)
}


fun AppCompatActivity.replace_fragment(fragment: Fragment, addStack: Boolean = true) {
    if (addStack) {
        supportFragmentManager.beginTransaction()
            .addToBackStack(null)
            .replace(
                R.id.data_container,
                fragment
            ).commit()
    } else {
        supportFragmentManager.beginTransaction()
            .replace(
                R.id.data_container,
                fragment
            ).commit()
    }
}

fun Fragment.replace_fragment(fragment: Fragment) {
    this.fragmentManager?.beginTransaction()
        ?.addToBackStack(null)
        ?.replace(
            R.id.data_container,
            fragment
        )?.commit()
}

fun Fragment.get_filename_from_uri(uri: Uri): String {
    var result = ""
    val cursor = MAIN_ACTIVITY.contentResolver.query(uri, null, null, null, null)
    try {
        if (cursor != null && cursor.moveToFirst()) {
            result = cursor.getString(cursor.getColumnIndex(OpenableColumns.DISPLAY_NAME))
        }
    } catch (e: Exception) {
        show_toast(e.message.toString())
    } finally {
        cursor?.close()
        return result
    }
}

fun hide_key_board() {
    val imm: InputMethodManager =
        MAIN_ACTIVITY.getSystemService(AppCompatActivity.INPUT_METHOD_SERVICE) as InputMethodManager
    imm.hideSoftInputFromWindow(MAIN_ACTIVITY.window.decorView.windowToken, 0)
}

fun Fragment.set_string(editText: EditText): String {
    if (editText.text.isNullOrEmpty()) {
        show_toast(getString(R.string.set_string_err))
        return ""
    } else {
        show_toast(getString(R.string.sett_string_ok))
        return editText.text.toString()
    }
}

fun Fragment.is_user_data_null(user: this_user): Boolean {
    if (user.Name.isEmpty() || user.Surname.isEmpty() || user.Midname.isEmpty() || user.Group.isEmpty())
        return false
    return true
}

fun ImageView.set_image(url: String) {
    Picasso.get()
        .load(url)
        .fit()
        .placeholder(R.drawable.ico_student)
        .into(this)
}