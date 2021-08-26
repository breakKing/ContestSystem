<template>
  <div class="flex-shrink-0 side-bar-menu main-block-height d-flex flex-column justify-content-start"
       style="height: 100%; padding: 10px">

    <h4 class="fw-bold fs-3 m-2 mb-5 text-light">Панель модератора</h4>

    <ul class="list-unstyled ps-0 ">
<!--      <li class="mb-1">-->
<!--        <button class="btn btn-custom btn-toggle align-items-center rounded collapsed text-light" data-bs-toggle="collapse"-->
<!--                data-bs-target="#courses-collapse" aria-expanded="false">-->
<!--          Курсы-->
<!--        </button>-->
<!--        <div class="collapse"-->
<!--             :class="{show: ['ModeratorNotModeratedCoursesPage', 'ModeratorApprovedCoursesPage', 'ModeratorRejectedCoursesPage'].includes($route.name)}"-->
<!--             id="courses-collapse">-->
<!--          <ul class="btn-toggle-nav list-unstyled fw-normal pb-1 small">-->
<!--            <li>-->
<!--              <router-link class="link-light rounded" :to="{name: 'ModeratorNotModeratedCoursesPage'}">Ожидающие-->
<!--                модерации-->
<!--              </router-link>-->
<!--            </li>-->
<!--            <li>-->
<!--              <router-link class="link-light rounded" :to="{name: 'ModeratorApprovedCoursesPage'}">Одобренные-->
<!--              </router-link>-->
<!--            </li>-->
<!--            <li>-->
<!--              <router-link class="link-light rounded" :to="{name: 'ModeratorRejectedCoursesPage'}">Отклонённые-->
<!--              </router-link>-->
<!--            </li>-->
<!--          </ul>-->
<!--        </div>-->
<!--      </li>-->
      <li class="mb-1">
        <button class="btn btn-custom btn-toggle align-items-center rounded collapsed text-light" data-bs-toggle="collapse"
                data-bs-target="#posts-collapse" aria-expanded="false">
          Посты
        </button>
        <div class="collapse"
             :class="{show: ['ModeratorNotModeratedPostsPage', 'ModeratorApprovedPostsPage', 'ModeratorRejectedPostsPage'].includes($route.name)}"
             id="posts-collapse">
          <ul class="btn-toggle-nav list-unstyled fw-normal pb-1 small">
            <li>
              <router-link class="link-light rounded" :to="{name: 'ModeratorNotModeratedPostsPage'}">Ожидающие модерации
              </router-link>
            </li>
            <li>
              <router-link class="link-light rounded" :to="{name: 'ModeratorApprovedPostsPage'}">Одобренные
              </router-link>
            </li>
            <li>
              <router-link class="link-light rounded" :to="{name: 'ModeratorRejectedPostsPage'}">Отклонённые
              </router-link>
            </li>
          </ul>
        </div>
      </li>
      <li class="mb-1">
        <button class="btn btn-custom btn-toggle align-items-center rounded collapsed text-light" data-bs-toggle="collapse"
                data-bs-target="#contests-collapse" aria-expanded="false">
          Соревнования
        </button>
        <div class="collapse"
             :class="{show: ['ModeratorNotModeratedContestsPage', 'ModeratorApprovedContestsPage', 'ModeratorRejectedContestsPage'].includes($route.name)}"
             id="contests-collapse">
          <ul class="btn-toggle-nav list-unstyled fw-normal pb-1 small">
            <li>
              <router-link class="link-light rounded" :to="{name: 'ModeratorNotModeratedContestsPage'}">Ожидающие
                модерации
              </router-link>
            </li>
            <li>
              <router-link class="link-light rounded" :to="{name: 'ModeratorApprovedContestsPage'}">Одобренные
              </router-link>
            </li>
            <li>
              <router-link class="link-light rounded" :to="{name: 'ModeratorRejectedContestsPage'}">Отклонённые
              </router-link>
            </li>
          </ul>
        </div>
      </li>
      <li class="mb-1">
        <button class="btn btn-custom btn-toggle align-items-center rounded collapsed text-light" data-bs-toggle="collapse"
                data-bs-target="#tasks-collapse" aria-expanded="false">
          Задачи
        </button>
        <div class="collapse"
             :class="{show: ['ModeratorNotModeratedProblemsPage', 'ModeratorApprovedProblemsPage', 'ModeratorRejectedProblemsPage'].includes($route.name)}"
             id="tasks-collapse">
          <ul class="btn-toggle-nav list-unstyled fw-normal pb-1 small">
            <li>
              <router-link class="link-light rounded" :to="{name: 'ModeratorNotModeratedProblemsPage'}">Ожидающие
                модерации
              </router-link>
            </li>
            <li>
              <router-link class="link-light rounded" :to="{name: 'ModeratorApprovedProblemsPage'}">Одобренные
              </router-link>
            </li>
            <li>
              <router-link class="link-light rounded" :to="{name: 'ModeratorRejectedProblemsPage'}">Отклонённые
              </router-link>
            </li>
          </ul>
        </div>
      </li>
      <li class="mb-1">
        <button class="btn btn-custom btn-toggle align-items-center rounded collapsed text-light" data-bs-toggle="collapse"
                data-bs-target="#checkers-collapse" aria-expanded="false">
          Чекеры
        </button>
        <div class="collapse"
             :class="{show: ['ModeratorNotModeratedCheckersPage', 'ModeratorApprovedCheckersPage', 'ModeratorRejectedCheckersPage'].includes($route.name)}"
             id="checkers-collapse">
          <ul class="btn-toggle-nav list-unstyled fw-normal pb-1 small">
            <li>
              <router-link class="link-light rounded" :to="{name: 'ModeratorNotModeratedCheckersPage'}">Ожидающие
                модерации
              </router-link>
            </li>
            <li>
              <router-link class="link-light rounded" :to="{name: 'ModeratorApprovedCheckersPage'}">Одобренные
              </router-link>
            </li>
            <li>
              <router-link class="link-light rounded" :to="{name: 'ModeratorRejectedCheckersPage'}">Отклонённые
              </router-link>
            </li>
          </ul>
        </div>
      </li>
      <li class="mb-1">
        <button class="btn btn-custom btn-toggle align-items-center rounded collapsed text-light" data-bs-toggle="collapse"
                data-bs-target="#rule-sets-collapse" aria-expanded="false">
          Наборы правил
        </button>
        <div class="collapse"
             :class="{show: ['ModeratorNotModeratedRuleSetsPage', 'ModeratorApprovedRuleSetsPage', 'ModeratorRejectedRuleSetsPage'].includes($route.name)}"
             id="rule-sets-collapse">
          <ul class="btn-toggle-nav list-unstyled fw-normal pb-1 small">
            <li>
              <router-link class="link-light rounded" :to="{name: 'ModeratorNotModeratedRuleSetsPage'}">Ожидающие
                модерации
              </router-link>
            </li>
            <li>
              <router-link class="link-light rounded" :to="{name: 'ModeratorApprovedRuleSetsPage'}">Одобренные
              </router-link>
            </li>
            <li>
              <router-link class="link-light rounded" :to="{name: 'ModeratorRejectedRuleSetsPage'}">Отклонённые
              </router-link>
            </li>
          </ul>
        </div>
      </li>
    </ul>
  </div>
</template>

<script>
export default {
  name: "ModerSideBarComponent"
}
</script>

<style lang="scss" scoped>

</style>