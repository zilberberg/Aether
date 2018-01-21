using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SampleDatabase : MonoBehaviour 
{
	public string serverMainname = "localhost";
	public string serverUsername = "root";
	public string serverPassword = "";
	public string serverDatabase = "developer";

	public string targetPhp = "http://localhost/server/PrimaryDatabaseDeveloper.php";

	private WWWForm Authenticate()
	{
		WWWForm form = new WWWForm ();
		form.AddField("serverMainname", serverMainname);
		form.AddField("serverUsername", serverUsername);
		form.AddField("serverPassword", serverPassword);
		form.AddField("serverDatabase", serverDatabase);
		return form;
	}

	private void DebugResult(string result)
	{
		string[] data = result.Split (new char[] { '|' });
		foreach(string text in data)
		{
			Debug.LogWarning (text);
		}
	}

	// DEVELOPER METHODS!

	public void DatabaseCreate(string databaseName)
	{
		StartCoroutine(CreateDatabase(databaseName));
	}

	private IEnumerator CreateDatabase(string databaseName)
	{
		WWWForm form = Authenticate ();
		form.AddField("methodName", "DatabaseCreate");
		form.AddField("databaseName", databaseName);
	
		WWW web = new WWW (targetPhp, form);
		yield return web;
		DebugResult (web.text);
	}

	public void DatabaseDelete(string databaseName)
	{
		StartCoroutine(DeleteDatabase(databaseName));
	}

	private IEnumerator DeleteDatabase(string databaseName)
	{
		WWWForm form = Authenticate ();
		form.AddField("methodName", "DatabaseDelete");
		form.AddField("databaseName", databaseName);

		WWW web = new WWW (targetPhp, form);
		yield return web;
		DebugResult (web.text);
	}

	public void DatabaseList()
	{
		StartCoroutine(ListDatabase());
	}

	private IEnumerator ListDatabase()
	{
		WWWForm form = Authenticate ();
		form.AddField("methodName", "DatabaseList");

		WWW web = new WWW (targetPhp, form);
		yield return web;
		DebugResult (web.text);
	}

	public void TableCreate(string tableName)
	{
		StartCoroutine(CreateTable(tableName));
	}

	private IEnumerator CreateTable(string tableName)
	{
		WWWForm form = Authenticate ();
		form.AddField("methodName", "TableCreate");
		form.AddField("tableName", tableName);
		form.AddField("databaseName", serverDatabase);

		string tableContent = 
			"username VARCHAR(12) NOT NULL , " +	//test name of content!
			"simple VARCHAR(240) NOT NULL , " +		//test name of content!
			"advance VARCHAR(4800) NOT NULL , " ;	//test name of content!

		form.AddField("tableContent", tableContent); //test name of database!

		WWW web = new WWW (targetPhp, form);
		yield return web;
		DebugResult (web.text);
	}

	public void TableAlterAdd(string tableName)
	{
		StartCoroutine(AlterAddTable(tableName));
	}

	private IEnumerator AlterAddTable(string tableName)
	{
		WWWForm form = Authenticate ();
		form.AddField("methodName", "TableAlterAdd");
		form.AddField("tableName", tableName);
		form.AddField("databaseName", "userdatabase"); //test name of database!

		string newContent = "newcontent VARCHAR(1200) NOT NULL" ;	//test name of content!
		form.AddField("newContent", newContent);

		WWW web = new WWW (targetPhp, form);
		yield return web;
		DebugResult (web.text);
	}

	public void TableAlterChange(string tableName)
	{
		StartCoroutine(AlterChangeTable(tableName));
	}

	private IEnumerator AlterChangeTable(string tableName)
	{
		WWWForm form = Authenticate ();
		form.AddField("methodName", "TableAlterChange");
		form.AddField("tableName", tableName);
		form.AddField("databaseName", "userdatabase"); //test name of database!

		string oldContent = "advance" ;	//test name of content! old
		form.AddField("oldContent", oldContent);

		string newContent = "advances" ;	//test name of content! new
		form.AddField("newContent", newContent);

		string newLength = "4800" ;	//test name of content! new
		form.AddField("newLength", newLength);

		WWW web = new WWW (targetPhp, form);
		yield return web;
		DebugResult (web.text);
	}

	public void TableAlterDelete(string tableName)
	{
		StartCoroutine(AlterDeleteTable(tableName));
	}

	private IEnumerator AlterDeleteTable(string tableName)
	{
		WWWForm form = Authenticate ();
		form.AddField("methodName", "TableAlterDelete");
		form.AddField("tableName", tableName);
		form.AddField("databaseName", "userdatabase"); //test name of database!

		string deleteContent = "newcontent" ;	//test name of content!
		form.AddField("deleteContent", deleteContent);

		WWW web = new WWW (targetPhp, form);
		yield return web;
		DebugResult (web.text);
	}

	public void TableDelete(string tableName)
	{
		StartCoroutine(DeleteTable(tableName));
	}

	private IEnumerator DeleteTable(string tableName)
	{
		WWWForm form = Authenticate ();
		form.AddField("methodName", "TableDelete");
		form.AddField("tableName", tableName);
		form.AddField("databaseName", "userdatabase"); //test name of database!

		WWW web = new WWW (targetPhp, form);
		yield return web;
		DebugResult (web.text);
	}

	public void TableList(string databaseName)
	{
		StartCoroutine(ListTable(databaseName));
	}

	private IEnumerator ListTable(string databaseName)
	{
		WWWForm form = Authenticate ();
		form.AddField("methodName", "TableList");
		form.AddField("databaseName", databaseName);

		WWW web = new WWW (targetPhp, form);
		yield return web;
	}
}
