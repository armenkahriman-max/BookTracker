# User perspective

1. Long book titles overlap / get smashed together in the list view → looks broken and hard to read.
2. Free (non-logged-in) users already have full access to the book list + search by title/author. Creating an account therefore feels pointless for normal users.
3. No visible difference or extra value for a registered member compared with a free visitor (beyond what an admin can do).


# Developer / Admin perspective

4. When adding a book, the year field accepts completely invalid values 
   (e.g. negative years like -01231). No validation.
5. Admin can permanently delete members (and even their own account). There is no temporary ban or permanent ban option — only hard delete.
6. There is no profanity filter on any text fields (book titles, member names, etc.). Offensive content can be entered freely.
7. Admin can already: view own account info, CRUD books, search/list members (with pagination), update members, update own account, delete members.

 
#  Finding                 Value       Effort         Risk         Quick note
1.Long titles              High         Low           Low          Pure UI fix 
   overlap                          

2.Free users have        High         Medium        Medium       Needs a real benefit for
same access as members                                              logged-in users
no reason to register

3.Year field accepts       Medium       Low           Low          Simple validation (min/max year)
  nonsense values

4.Only hard-delete,
no temp/permanent ban      High        Medium         Medium       Needs new status field + logic

5.No profanity/            Medium       Medium        Low-Medium    Depends how advanced the filter is
insult filter

# Selected improvements: 
1.Fix long title display  
2.Add proper year validation when adding/editing books  
3.Give registered members real extra value so creating an account makes sense  
4.Give admins temporary ban + permanent ban options

# Motivation:
We focus on the highest-value items that are realistic to implement.
Quick visual and data-quality fixes come first (they make the app look and feel professional immediately).
Then we solve the two structural problems that hurt the product most: lack of membership value and weak moderation tools.
The profanity filter is useful but can wait — it is more complex and lower priority than the four chosen items.

# Desired result
1. Long book titles
Titles never overlap or get cut off in a broken way.  
Long titles are either truncated with “…” or wrap cleanly to the next line.  
The list stays readable and looks professional on all screen sizes.

2. Year validation
When adding or editing a book, the year field only
accepts realistic values (for example 1000–current year + 1).  
Negative numbers, letters, or absurd years are rejected with a clear error message.  
Existing invalid data can still be viewed, but new bad data cannot be saved.

3. Real value for registered members
Free (not logged-in) users can still browse and search the public book list.  
Logged-in members get at least one clear extra feature, for example:
– Ability to mark books as “Want to read / Reading / Finished”
– Personal reading list / favourites
– Or simple personal notes on books  
The difference is obvious, so users understand why they should create an account.

4. Ban instead of only delete
Admin can temporarily ban a member (for a chosen number of days) or permanently ban them.  
Banned users cannot log in.  
The member record stays in the system (with a clear “Banned” status) so the admin can unban later if needed.  
Hard delete remains available only as a last-resort option (or is removed completely for safety).  
Admin can no longer accidentally delete their own account.

    # Proposed order of implementation
Fix long title display (quick visual win, zero risk)  
Add year validation (protects data quality, still very easy)  
Implement temporary + permanent ban system (improves moderation safety)  
Add real member-only features (gives purpose to registration)

Why this order? 
Start with the easiest, most visible improvements
you see progress fast and the app already looks better.  

Fix data validation early so you don’t keep adding bad books while working on other features.

Ban system next because it is important for safety and does not depend on the membership features.  

Membership value last because it may touch more parts of the application (UI + database + permissions).

5. # Most important considerations 
Keep it simple. Every change should be as small as possible while still solving the problem.  

Don’t break existing functionality. Test the book list, search, and admin actions after every change. 

Safety first for the ban feature.
Make sure an admin cannot lock themselves out permanently.  
User communication. When a free user tries to use a member-only feature, show a clear message: “Create a free account to unlock this.”  

Future-proof. The ban status and member features should be designed so a profanity filter or more advanced roles can be added later without rewriting everything.  

No over-engineering. We are not building a full Goodreads clone — just enough extra value that registration makes sense.











