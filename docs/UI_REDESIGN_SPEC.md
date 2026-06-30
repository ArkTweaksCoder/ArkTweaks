# Ark Optimizer UI Redesign Specification

Version: 1.0

Author: Ark Tweaks

Status: Approved

---

# Vision

Ark Optimizer should feel like a premium desktop application designed for gamers and power users.

The application must NOT resemble:

- Windows Control Panel
- Legacy WinForms software
- Generic WPF templates
- Basic admin dashboards
- "Optimizer" scamware

Instead it should feel similar to the polish found in:

• Discord
• Steam
• Arc Browser
• Windows 11
• Spotify Desktop
• Notion
• Adobe Creative Cloud
• SteelSeries GG
• Logitech G HUB (layout inspiration only)

The application should immediately communicate:

Professional

Modern

Fast

Safe

Minimal

Premium

Gaming-focused

---

# Overall Design Language

Design Philosophy:

Modern glass-inspired desktop UI

Dark-first

Rounded

Card-based

Layered

Minimal clutter

Lots of whitespace

Motion-rich

Soft shadows

Subtle depth

Everything should feel smooth.

Nothing should feel abrupt.

---

# Color System

Primary Background

#090B10

Secondary Background

#10141B

Sidebar

#0D1016

Cards

#151B23

Elevated Cards

#1B2330

Accent

#4F8CFF

Accent Hover

#6AA5FF

Accent Pressed

#3B75E6

Success

#38D27A

Warning

#F6B44A

Danger

#FF5A67

Info

#56B6FF

Primary Text

#FFFFFF

Secondary Text

#B8C0CC

Muted Text

#7C8798

Borders

rgba(255,255,255,0.06)

Separators

rgba(255,255,255,0.04)

Hover

rgba(255,255,255,0.03)

Pressed

rgba(255,255,255,0.08)

---

# Typography

Font:

Segoe UI Variable

Fallback:

Inter

Titles

30px

Bold

Section Headers

22px

SemiBold

Card Titles

18px

SemiBold

Normal Text

15px

Medium

Small Labels

13px

Regular

Metric Numbers

36px

Bold

Large Health Score

64px

ExtraBold

Numbers should use tabular figures when possible.

---

# Border Radius

Buttons

12px

Cards

18px

Dialogs

22px

Sidebar Elements

14px

Input Boxes

10px

Toast Notifications

16px

---

# Shadows

Cards

Very soft

Blurred

Low opacity

Hover

Slightly larger shadow

Pressed

Reduced shadow

Never use harsh shadows.

---

# Window

Custom title bar

Rounded corners

Custom minimize/maximize/close

Custom app icon

Resizable

Minimum width

1280

Minimum height

820

Smooth resizing

---

# Sidebar

Width

260px

Contains

Logo

Application Name

Version

Navigation

Bottom utility section

License badge

Settings shortcut

Discord button

Website button

Navigation Items

Dashboard

Optimize

Cleanup

Startup

Gaming

Performance

Restore

History

Logs

Settings

About

Each item includes:

Icon

Label

Hover animation

Selection indicator

Glow effect

Animated transition

Selected page should display:

Animated accent bar

Background highlight

Glow

---

# Dashboard

Layout

Scrollable

Responsive

Card-based

Top Row

Health Score

CPU

GPU

RAM

Disk

Power Plan

Windows Version

Second Row

Quick Actions

Optimize

Cleanup

Refresh

Restore Point

Third Row

Recommendations

Priority

Impact

Action

Fourth Row

Performance History Preview

Health Trend

Cleanup Trend

Storage Trend

Bottom

Recent Activity

Logs

Timeline

Status

Every card should animate on hover.

---

# Cards

Padding

24px

Rounded

18px

Animated

Hover lift

Glow border

Soft shadow

Card Header

Icon

Title

Description

Card Body

Metrics

Charts

Buttons

Card Footer

Timestamp

Status

---

# Buttons

Primary

Blue

Rounded

Subtle shadow

Hover scale

Hover glow

Pressed animation

Secondary

Dark

Border

Light hover

Danger

Red

Confirmation required

Icon Buttons

Circular

Hover glow

Tooltip

---

# Inputs

Rounded

Soft background

Accent focus border

Animated placeholder

Validation states

Success

Warning

Error

---

# Tables

Modern

Rounded rows

Hover highlight

Sticky headers

Alternating row opacity

Search bar

Sorting

Filtering

Column resizing

---

# Progress Bars

Rounded

Animated fill

Gradient

Percentage

Glow

---

# Charts

Smooth lines

Rounded

Animated loading

Minimal grid

Tooltip

Legends

Future-ready

---

# Recommendation Cards

Priority Badge

Estimated Impact

Estimated Time

Risk

Description

Action Button

Dismiss Button

Hover animation

---

# Cleanup Page

Left

Categories

Right

Files

Bottom

Estimated reclaimed space

Top

Scan

Clean Selected

Clean All

Progress

Animated scan

Animated cleanup

Completion celebration

---

# Startup Manager

Search

Sort

Publisher

Impact

Status

Enable

Disable

Filter

Microsoft

Third-party

Gaming

Utilities

---

# Performance Page

Live metrics

CPU graph

RAM graph

Disk graph

Power Plan

Windows Uptime

Temperature placeholders

Future graph support

---

# History

Timeline

Charts

Optimization history

Cleanup history

Recommendation history

---

# Settings

Category navigation

Cards

Toggle switches

Dropdowns

Sliders

Instant save

Reset buttons

Search

---

# About

Large logo

Version

License

Credits

Website

Discord

GitHub

---

# Toast Notifications

Top-right

Slide animation

Fade

Icons

Auto dismiss

Success

Warning

Error

Information

---

# Dialogs

Blur background

Rounded

Animated open

Animated close

Confirmation dialogs

Danger dialogs

Success dialogs

---

# Loading

Skeleton loaders

Spinner

Progress indicator

Fade transitions

No blocking UI

---

# Animations

Page transitions

250ms

Button hover

150ms

Card hover

150ms

Sidebar selection

250ms

Dialogs

200ms

Toast

300ms

Loading

Continuous

Animations should use easing curves.

Never linear.

---

# Icons

Use Fluent UI icons.

Consistent sizing.

Outlined style.

Accent icons where appropriate.

---

# Accessibility

Keyboard navigation

Focus indicators

Screen reader labels

High contrast compatibility

Large click targets

---

# Responsiveness

Support:

1280x820

1440p

4K scaling

Windows display scaling

No overlapping elements.

No clipped content.

---

# Performance

Maintain 60 FPS UI rendering.

Avoid layout thrashing.

Virtualize long lists.

Lazy load heavy pages.

---

# Overall Goal

When a user opens Ark Optimizer, the immediate impression should be:

"This looks like software from a professional company."

Every interaction should reinforce:

Fast

Modern

Safe

Premium

Gaming-focused

Trustworthy

Never resemble a generic WPF application.